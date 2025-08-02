import { inject } from '@angular/core';
import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, switchMap, throwError, Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService } from '../services/common/notification-service';
import { SessionService } from '../services/common/session-service';
import { AccountApiService } from '../services/api/account-api-service';
import { TokensForRefreshDTO } from '../interfaces/tokens-for-refresh-dto';

let isRefreshing = false;
const refreshSubject = new Subject<string>();

export const HTTPInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const translateService = inject(TranslateService);
  const notificationService = inject(NotificationService);
  const sessionService = inject(SessionService);
  const accountApiService = inject(AccountApiService);
  const accessToken = sessionService.getAccessToken();

  const authRequest = accessToken
    ? request.clone({
        headers: request.headers.set('Authorization', `Bearer ${accessToken}`)
      })
    : request;

  return next(authRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !authRequest.headers.has('X-Retry')) {
        if (!isRefreshing) {
          isRefreshing = true;

          const refreshToken = sessionService.getRefreshToken();
          const oldAccessToken = sessionService.getAccessToken();

          const tokensForRefreshDTO: TokensForRefreshDTO = { accessToken: oldAccessToken, refreshToken: refreshToken };

          return accountApiService.refreshTokens(tokensForRefreshDTO).pipe(switchMap((tokens) => {
              sessionService.setAccessToken(tokens.accessToken);
              sessionService.setRefreshToken(tokens.refreshToken);
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
              sessionService.deleteTokens();
              notificationService.showErrorToast('YOUR_SESSION_HAS_EXPIRED');
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
      if (error.status >= 400 && error.status !== 401) {
        const errorKey = error?.error?.message;
        if (errorKey && typeof errorKey === 'string') {
          const translationKey = `ERROR.${errorKey}`;
          translateService.get(translationKey).subscribe((translated) => {
            if (translated === translationKey) {
              notificationService.showErrorToast('AN_UNKNOWN_ERROR_OCCURED');
            }
            else {
              notificationService.showErrorToast(translationKey);
            }
          });
        }
        else {
          notificationService.showErrorToast('AN_UNKNOWN_ERROR_OCCURED');
        }
      }

      return throwError(() => error);
    })
  );
};