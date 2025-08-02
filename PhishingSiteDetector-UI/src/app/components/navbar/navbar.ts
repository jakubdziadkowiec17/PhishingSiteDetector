import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Menubar } from 'primeng/menubar';
import { BadgeModule } from 'primeng/badge';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../services/common/account-service';
import { RouterModule } from '@angular/router';
import { AccountApiService } from '../../services/api/account-api-service';
import { RefreshTokenDTO } from '../../interfaces/refresh-token-dto';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
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
  private langChangeSub?: Subscription;
  items: MenuItem[] = [];

  constructor(private accountService: AccountService, private accountApiService: AccountApiService, private translateService: TranslateService, private notificationService: NotificationService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.accountSub = this.accountService.account$.subscribe(account => {
      this.account = account;
      this.buildNavbar();
    });

    this.langChangeSub = this.translateService.onLangChange.subscribe(() => {
      this.buildNavbar();
    });
  }

  ngOnDestroy() {
    this.accountSub?.unsubscribe();
    this.langChangeSub?.unsubscribe();
  }

  buildNavbar() {
    const isAuth = this.accountService.isAuthenticated();
    const currentLanguage = this.translateService.getCurrentLang();

    this.translateService.get([
      'NAVBAR.HOME',
      'NAVBAR.LANGUAGES',
      'NAVBAR.SIGN_IN',
      'NAVBAR.STATISTICS',
      'NAVBAR.DATA_SETS',
      'NAVBAR.SETTINGS',
      'NAVBAR.SIGN_OUT'
    ]).subscribe(translations => {
      const homeItem = {
        label: translations['NAVBAR.HOME'],
        icon: 'pi pi-home',
        routerLink: ['/']
      };

      const languageItem = {
        label: translations['NAVBAR.LANGUAGES'],
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
      };

      if (!isAuth) {
        this.items = [
          homeItem,
          languageItem,
          {
            label: translations['NAVBAR.SIGN_IN'],
            icon: 'pi pi-sign-in',
            styleClass: 'sign-in',
            routerLink: ['/login']
          }
        ];
      }
      else {
        this.items = [
          homeItem,
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
          languageItem,
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
                label: translations['NAVBAR.SIGN_OUT'],
                icon: 'pi pi-sign-out',
                command: () => this.logout()
              }
            ]
          }
        ];
      }

      this.cdr.detectChanges();
    });
  }

  private changeLanguage(languageCode: string): void {
    if(Languages.includes(languageCode)) {
      this.accountService.setLanguageCode(languageCode);
      const isAuth = this.accountService.isAuthenticated();
      if(isAuth) {
        const languageDTO: LanguageDTO = { languageCode: languageCode };
        this.accountApiService.changeLanguage(languageDTO).subscribe();
      }
    }
    else {
      this.notificationService.showErrorToast('SELECTED_LANGUAGE_DOES_NOT_EXISTS_IN_THE_APP');
    }
  }

  private logout(): void {
    const refreshToken = this.accountService.getRefreshToken();
    const refreshTokenDTO: RefreshTokenDTO = {refreshToken: refreshToken};

    this.accountApiService.logout(refreshTokenDTO).subscribe({
      next: (response) => {
        this.accountService.deleteTokensWithRedirect();
        this.notificationService.showSuccessToast(response.message);
      }
    });
  }
}