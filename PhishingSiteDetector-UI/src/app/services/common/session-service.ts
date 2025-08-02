import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  constructor(private cookieService: CookieService, private translateService: TranslateService) {}

  isAuthenticated(): boolean {
    return this.cookieService.check('AccessToken');
  }

  getAccessToken(): string | null {
    return this.cookieService.get('AccessToken') || null;
  }

  getRefreshToken(): string | null {
    return this.cookieService.get('RefreshToken') || null;
  }

  getLanguageCode(): string | null {
    return this.cookieService.get('LanguageCode') || null;
  }
  
  setAccessToken(value: string) {
    this.cookieService.set('AccessToken', value);
  }

  setRefreshToken(value: string) {
    this.cookieService.set('RefreshToken', value);
  }

  setLanguageCode(value: string) {
    this.cookieService.set('LanguageCode', value);
    this.translateService.use(value);
  }

  deleteTokens(): void {
    this.cookieService.delete('AccessToken');
    this.cookieService.delete('RefreshToken');
  }
}