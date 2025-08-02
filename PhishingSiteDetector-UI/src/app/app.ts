import { Component, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { Navbar } from "./components/navbar/navbar";
import { Footer } from "./components/footer/footer";
import { TranslateService } from '@ngx-translate/core';
import { AccountStoreService } from './services/store/account-store-service';
import { SessionService } from './services/common/session-service';
import { LanguageCode } from './constants/languageCode';
import { take } from 'rxjs';
import { Languages } from './constants/languages';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastModule, Navbar, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  constructor(private sessionService: SessionService, private translateService: TranslateService, private accountStoreService: AccountStoreService) {}

  ngOnInit() {
    this.translateService.addLangs(Languages);
    this.translateService.setDefaultLang(LanguageCode.EN);

    const isAuth = this.sessionService.isAuthenticated();

    if (isAuth) {
      this.accountStoreService.loadUser();

      this.accountStoreService.account$.pipe(take(1)).subscribe(account => {
        if (account && account.languageCode && Languages.includes(account.languageCode)) {
          this.sessionService.setLanguageCode(account.languageCode);
        }
        else {
          this.sessionService.setLanguageCode(LanguageCode.EN);
        }
      });
    }
    else {
      const languageCodeFromCookies = this.sessionService.getLanguageCode();
      const languageCode = languageCodeFromCookies && Languages.includes(languageCodeFromCookies) ? languageCodeFromCookies  : LanguageCode.EN;
      this.sessionService.setLanguageCode(languageCode);
    }
  }
}