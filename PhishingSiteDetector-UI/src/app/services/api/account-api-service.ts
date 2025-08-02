import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Server } from '../../constants/server';
import { LoginDTO } from '../../interfaces/login-dto';
import { TokensDTO } from '../../interfaces/tokens-dto';
import { AccountDataDTO } from '../../interfaces/account-data-dto';
import { AccountDTO } from '../../interfaces/account-dto';
import { ResetPasswordDTO } from '../../interfaces/reset-password-dto';
import { RefreshTokenDTO } from '../../interfaces/refresh-token-dto';
import { LanguageDTO } from '../../interfaces/language-dto';
import { TokensForRefreshDTO } from '../../interfaces/tokens-for-refresh-dto';
import { ResponseDTO } from '../../interfaces/response-dto';

@Injectable({
  providedIn: 'root'
})
export class AccountApiService {
  private readonly baseUrl = Server.apiUrl + 'account';

  constructor(private http: HttpClient) {}

  login(loginDTO: LoginDTO): Observable<TokensDTO> {
    return this.http.post<TokensDTO>(`${this.baseUrl}/login`, loginDTO);
  }

  refreshTokens(tokensForRefreshDTO: TokensForRefreshDTO): Observable<TokensDTO> {
    return this.http.post<TokensDTO>(`${this.baseUrl}/refresh-tokens`, tokensForRefreshDTO);
  }

  getAccountData(): Observable<AccountDataDTO> {
    return this.http.get<AccountDataDTO>(`${this.baseUrl}/data`);
  }

  getAccount(): Observable<AccountDTO> {
    return this.http.get<AccountDTO>(`${this.baseUrl}`);
  }

  editAccount(accountDTO: AccountDTO): Observable<ResponseDTO> {
    return this.http.put<ResponseDTO>(`${this.baseUrl}`, accountDTO);
  }

  changeLanguage(languageDTO: LanguageDTO): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/change-language`, languageDTO);
  }

  resetPassword(resetPasswordDTO: ResetPasswordDTO): Observable<ResponseDTO> {
    return this.http.put<ResponseDTO>(`${this.baseUrl}/reset-password`, resetPasswordDTO);
  }

  logout(refreshTokenDTO: RefreshTokenDTO): Observable<ResponseDTO> {
    return this.http.post<ResponseDTO>(`${this.baseUrl}/logout`, refreshTokenDTO);
  }
}