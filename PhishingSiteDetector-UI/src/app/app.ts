import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { Navbar } from "./components/navbar/navbar";
import { Footer } from "./components/footer/footer";
import { TranslateService } from '@ngx-translate/core';
import { AccountStoreService } from './services/store/account-store-service';
import { AccountService } from './services/common/account-service';
import { LanguageCode } from './constants/languageCode';
import { take } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastModule, Navbar, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  constructor(private accountService: AccountService, private translateService: TranslateService, private accountStoreService: AccountStoreService) {}

  ngOnInit() {
    this.translateService.addLangs([LanguageCode.EN, LanguageCode.PL]);
    this.translateService.setDefaultLang(LanguageCode.EN);

    const isAuth = this.accountService.isAuthenticated();

    if (isAuth) {
      this.accountStoreService.loadUser();

      this.accountStoreService.account$.pipe(take(1)).subscribe(account => {
        if (account && account.languageCode) {
          this.accountService.setLanguageCode(account.languageCode);
        }
        else {
          this.accountService.setLanguageCode(LanguageCode.EN);
        }
      });
    }
    else {
      const languageCodeFromCookies = this.accountService.getLanguageCode();
      const languageCode = languageCodeFromCookies && [LanguageCode.EN.toString(), LanguageCode.PL.toString()].includes(languageCodeFromCookies) ? languageCodeFromCookies  : LanguageCode.EN;
      this.accountService.setLanguageCode(languageCode);
    }
  }
}