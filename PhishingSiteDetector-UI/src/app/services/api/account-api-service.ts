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

@Injectable({
  providedIn: 'root'
})
export class AccountApiService {
  private readonly baseUrl = Server.apiUrl + 'account';

  constructor(private http: HttpClient) {}

  login(loginDTO: LoginDTO): Observable<TokensDTO> {
    return this.http.post<TokensDTO>(`${this.baseUrl}/login`, loginDTO);
  }

  refreshTokens(tokensDTO: TokensDTO): Observable<TokensDTO> {
    return this.http.post<TokensDTO>(`${this.baseUrl}/refresh-tokens`, tokensDTO);
  }

  getAccountData(): Observable<AccountDataDTO> {
    return this.http.get<AccountDataDTO>(`${this.baseUrl}/data`);
  }

  getAccount(): Observable<AccountDTO> {
    return this.http.get<AccountDTO>(`${this.baseUrl}`);
  }

  editAccount(accountDTO: AccountDTO): Observable<string> {
    return this.http.put<string>(`${this.baseUrl}`, accountDTO);
  }

  resetPassword(resetPasswordDTO: ResetPasswordDTO): Observable<string> {
    return this.http.put<string>(`${this.baseUrl}/reset-password`, resetPasswordDTO);
  }

  logout(refreshTokenDTO: RefreshTokenDTO): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/logout`, refreshTokenDTO);
  }
}