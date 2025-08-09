import { inject } from '@angular/core';
import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, switchMap, throwError, Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService } from '../services/common/notification-service';
import { AccountService } from '../services/common/account-service';
import { AccountApiService } from '../services/api/account-api-service';
import { TokensForRefreshDTO } from '../interfaces/tokens-for-refresh-dto';

let isRefreshing = false;
const refreshSubject = new Subject<string>();

export const HTTPInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const translateService = inject(TranslateService);
  const notificationService = inject(NotificationService);
  const accountService= inject(AccountService);
  const accountApiService = inject(AccountApiService);
  const accessToken = accountService.getAccessToken();
  const isAuth = accountService.isAuthenticated();

  const authRequest = isAuth
    ? request.clone({
        headers: request.headers.set('Authorization', `Bearer ${accessToken}`)
      })
    : request;

  return next(authRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      const isLoginOrRefresh = request.url.includes('/login') || request.url.includes('/refresh-tokens');
      if (error.status === 401 && !authRequest.headers.has('X-Retry') && !isLoginOrRefresh) {
        if (!isRefreshing) {
          isRefreshing = true;
          const refreshToken = accountService.getRefreshToken();
          const oldAccessToken = accountService.getAccessToken();
          const tokensForRefreshDTO: TokensForRefreshDTO = { accessToken: oldAccessToken, refreshToken: refreshToken };

          return accountApiService.refreshTokens(tokensForRefreshDTO).pipe(switchMap((tokens) => {
              accountService.setAccessToken(tokens.accessToken);
              accountService.setRefreshToken(tokens.refreshToken);
              isRefreshing = false;
              refreshSubject.next(tokens.accessToken);

              const retryReq = request.clone({
                headers: request.headers
                  .set('Authorization', `Bearer ${tokens.accessToken}`)
                  .set('X-Retry', 'true')
              });

              return next(retryReq);
            }),
            catchError((refreshError) => {
              isRefreshing = false;
              if (refreshError.status === 401) accountService.deleteTokensWithRedirect();

              return throwError(() => refreshError);
            })
          );
        }
        else {
          return refreshSubject.pipe(
            switchMap((newAccessToken) => {
              const retryReq = request.clone({
                headers: request.headers
                  .set('Authorization', `Bearer ${newAccessToken}`)
                  .set('X-Retry', 'true')
              });

              return next(retryReq);
            })
          );
        }
      }
      else if (error.status >= 400) {
        notificationService.showErrorToast(error?.error?.message);
      }
      else {
        notificationService.showErrorToast('UNABLE_TO_CONNECT_TO_THE_APP_SERVER');
      }

      return throwError(() => error);
    })
  );
};