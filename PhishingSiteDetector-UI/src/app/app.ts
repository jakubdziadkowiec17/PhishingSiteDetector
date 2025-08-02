import { Component, OnDestroy, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { Navbar } from "./components/navbar/navbar";
import { Footer } from "./components/footer/footer";
import { TranslateService } from '@ngx-translate/core';
import { AccountService } from './services/common/account-service';
import { LanguageCode } from './constants/languageCode';
import { Subscription, take } from 'rxjs';
import { Languages } from './constants/languages';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastModule, Navbar, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  private accessTokenSub?: Subscription;

  constructor(private accountService: AccountService, private translateService: TranslateService) {}

  ngOnInit() {
    this.translateService.addLangs(Languages);
    this.translateService.setDefaultLang(LanguageCode.EN);

    this.accessTokenSub = this.accountService.accessToken$.subscribe(() => {
      this.accountService.initializeUserContext();
    });
  }

  ngOnDestroy() {
    this.accessTokenSub?.unsubscribe();
  }
}