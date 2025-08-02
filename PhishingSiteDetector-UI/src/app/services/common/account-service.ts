import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { jwtDecode } from 'jwt-decode';
import { CookieService } from 'ngx-cookie-service';
import { BehaviorSubject, Observable } from 'rxjs';
import { AccountDataDTO } from '../../interfaces/account-data-dto';
import { AccountApiService } from '../api/account-api-service';
import { Cookie } from '../../constants/cookie';
import { Claim } from '../../constants/claim';
import { Router } from '@angular/router';
import { Languages } from '../../constants/languages';
import { LanguageCode } from '../../constants/languageCode';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private accountSubject = new BehaviorSubject<AccountDataDTO | null>(null);
  public account$: Observable<AccountDataDTO | null> = this.accountSubject.asObservable();
  private accessTokenSubject = new BehaviorSubject<string | null>(null);
  accessToken$ = this.accessTokenSubject.asObservable();

  constructor(private accountApiService: AccountApiService, private cookieService: CookieService, private router: Router, private translateService: TranslateService) 
  {
    this.accessTokenSubject.next(this.getAccessToken());
  }

  get account(): AccountDataDTO | null {
    return this.accountSubject.value;
  }

  setAccount(account: AccountDataDTO | null): void {
    this.accountSubject.next(account);
  }

  initializeUserContext(): void {
    const isAuth = this.isAuthenticated();

    if (isAuth) {
      this.accountApiService.getAccountData().subscribe({
        next: (data) => {
          this.accountSubject.next(data);
          if (data && data.languageCode && Languages.includes(data.languageCode)) {
            this.setLanguageCode(data.languageCode);
          }
          else {
            this.setLanguageCode(LanguageCode.EN);
          }
        },
        error: () => this.accountSubject.next(null)
      });
    }
    else {
      const languageCodeFromCookies = this.getLanguageCode();
      const languageCode = languageCodeFromCookies && Languages.includes(languageCodeFromCookies) ? languageCodeFromCookies  : LanguageCode.EN;
      this.setLanguageCode(languageCode);
    }
  }

  isAuthenticated(): boolean {
    return this.cookieService.check(Cookie.AccessToken);
  }
  
  getUserId(): string | null {
    const accessToken = this.cookieService.get(Cookie.AccessToken);
    if (!accessToken) return null;
      
    try {
      const decoded = jwtDecode<{ [Claim.UserId]: string }>(accessToken);
      return decoded?.[Claim.UserId] ?? null;
    }
    catch {
      return null;
    }
  }
  
  getAccessToken(): string | null {
    return this.cookieService.get(Cookie.AccessToken) || null;
  }
  
  getRefreshToken(): string | null {
    return this.cookieService.get(Cookie.RefreshToken) || null;
  }
  
  getLanguageCode(): string | null {
    return this.cookieService.get(Cookie.LanguageCode) || null;
  }
    
  setAccessToken(value: string) {
    this.cookieService.set(Cookie.AccessToken, value);
    this.accessTokenSubject.next(value);
  }
  
  setRefreshToken(value: string) {
    this.cookieService.set(Cookie.RefreshToken, value);
  }
  
  setLanguageCode(value: string) {
    this.cookieService.set(Cookie.LanguageCode, value);
    this.translateService.use(value);

    const currentAccount = this.accountSubject.value;
    if (currentAccount) {
      const updatedAccount: AccountDataDTO = {
        ...currentAccount,
        languageCode: value
      };
      this.accountSubject.next(updatedAccount);
    }
  }
  
  deleteTokens(): void {
    this.cookieService.delete(Cookie.AccessToken);
    this.cookieService.delete(Cookie.RefreshToken);
    this.accessTokenSubject.next(null);
    this.accountSubject.next(null);
  }

  deleteTokensWithRedirect(): void {
    this.deleteTokens();
    this.router.navigate(['/login']);
  }
}