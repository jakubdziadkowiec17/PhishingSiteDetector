import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Menubar } from 'primeng/menubar';
import { BadgeModule } from 'primeng/badge';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { SessionService } from '../../services/common/session-service';
import { Router, RouterModule } from '@angular/router';
import { AccountApiService } from '../../services/api/account-api-service';
import { RefreshTokenDTO } from '../../interfaces/refresh-token-dto';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { AccountStoreService } from '../../services/store/account-store-service';
import { AccountDataDTO } from '../../interfaces/account-data-dto';
import { LanguageCode } from '../../constants/languageCode';
import { LanguageDTO } from '../../interfaces/language-dto';
import { Languages } from '../../constants/languages';
import { NotificationService } from '../../services/common/notification-service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [Menubar, BadgeModule, InputTextModule, CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class Navbar implements OnInit, OnDestroy {
  account: AccountDataDTO | null = null;
  private accountSub?: Subscription;
  items: MenuItem[] = [];
  langChangeSub: Subscription | undefined;

  constructor(private sessionService: SessionService, private accountApiService: AccountApiService, private router: Router, private translateService: TranslateService, private accountStoreService: AccountStoreService, private notificationService: NotificationService) {}

  ngOnInit() {
    this.accountSub = this.accountStoreService.account$.subscribe(account => {
      this.account = account;
    });
    
    this.buildMenu();

    this.langChangeSub = this.translateService.onLangChange.subscribe(() => {
      this.buildMenu();
    });
  }

  ngOnDestroy() {
    this.accountSub?.unsubscribe();
    this.langChangeSub?.unsubscribe();
  }
  
  buildMenu() {
    const isAuth = this.sessionService.isAuthenticated();
    const currentLanguage = this.translateService.getCurrentLang();

    this.translateService.get([
      'NAVBAR.HOME',
      'NAVBAR.LANGUAGE',
      'NAVBAR.SIGN_IN',
      'NAVBAR.STATISTICS',
      'NAVBAR.DATA_SETS',
      'NAVBAR.SETTINGS',
      'NAVBAR.SIGN_OUT'
    ]).subscribe(translations => {
      this.items = [
        {
          label: translations['NAVBAR.HOME'],
          icon: 'pi pi-home',
          routerLink: ['/']
        },
        {
          label: translations['NAVBAR.LANGUAGE'],
          icon: 'pi pi-globe',
          items: [
            {
              label: 'English',
              icon: currentLanguage === LanguageCode.EN ? 'pi pi-check' : '',
              styleClass: currentLanguage === LanguageCode.EN ? '' : 'other-language',
              command: () => this.changeLanguage(LanguageCode.EN)
            },
            {
              label: 'Polski',
              icon: currentLanguage === LanguageCode.PL ? 'pi pi-check' : '',
              styleClass: currentLanguage === LanguageCode.PL ? '' : 'other-language',
              command: () => this.changeLanguage(LanguageCode.PL)
            }
          ]
        },
        ...(!isAuth
          ? [
              {
                label: translations['NAVBAR.SIGN_IN'],
                icon: 'pi pi-sign-in',
                styleClass: 'sign-in',
                routerLink: ['/login']
              }
            ]
          : [
              {
                label: translations['NAVBAR.STATISTICS'],
                icon: 'pi pi-chart-bar',
                routerLink: ['/statistics']
              },
              {
                label: translations['NAVBAR.DATA_SETS'],
                icon: 'pi pi-database',
                routerLink: ['/data-sets']
              },
              {
                label: this.account?.firstName,
                icon: 'pi pi-user',
                items: [
                  {
                    label: translations['NAVBAR.SETTINGS'],
                    icon: 'pi pi-cog',
                    routerLink: ['/settings']
                  },
                  {
                    separator: true
                  },
                  {
                    label: translations['NAVBAR.SIGN_OUT'],
                    icon: 'pi pi-sign-out',
                    command: () => this.logout()
                  }
                ]
              }
            ])
      ];
    });
  }

  private changeLanguage(languageCode: string): void {
    if(Languages.includes(languageCode)) {
      this.sessionService.setLanguageCode(languageCode);
      const isAuth = this.sessionService.isAuthenticated();
      if(isAuth) {
        const languageDTO: LanguageDTO = { languageCode: languageCode };
        this.accountApiService.changeLanguage(languageDTO).subscribe({
          next: () => {
            const account = this.accountStoreService.account;
            if (account) {
              const updatedAccount = {
                ...account,
                languageCode: languageCode
              };
              this.accountStoreService.setAccount(updatedAccount);
            }
          }
        });
      }
    }
    else {
      this.notificationService.showErrorToast('SELECTED_LANGUAGE_DOES_NOT_EXISTS_IN_THE_APP');
    }
  }

  private logout(): void {
    const refreshToken = this.sessionService.getRefreshToken();
    const refreshTokenDTO: RefreshTokenDTO = {refreshToken: refreshToken};

      this.accountApiService.logout(refreshTokenDTO).subscribe({
        next: () => {
          this.sessionService.deleteTokens();
          this.router.navigate(['/login']);
        }
      });
  }
}