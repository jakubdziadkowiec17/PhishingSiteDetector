import { inject } from '@angular/core';
import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, from, switchMap, throwError, of, Subject } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

let isRefreshing = false;
const refreshSubject = new Subject<string>();

export const HTTPInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>,next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const cookieService = inject(CookieService);
  const http = inject(HttpClient);
  const messageService = inject(MessageService);
  const translate = inject(TranslateService);
  const accessToken = cookieService.get('AccessToken');

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

          const refreshToken = cookieService.get('RefreshToken');
          const oldAccessToken = cookieService.get('AccessToken');

          return http.post<{ accessToken: string; refreshToken: string }>('/api/auth/refresh',
              {
                accessToken: oldAccessToken,
                refreshToken: refreshToken
              }
            )
            .pipe(switchMap((tokens) => {
              cookieService.set('AccessToken', tokens.accessToken);
              cookieService.set('RefreshToken', tokens.refreshToken);
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
              cookieService.delete('AccessToken');
              cookieService.delete('RefreshToken');
              showErrorToast('YOUR_SESSION_HAS_EXPIRED');
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
          translate.get(translationKey).subscribe((translated) => {
            if (translated === translationKey) {
              showErrorToast('AN_UNKNOWN_ERROR_OCCURED');
            }
            else {
              showErrorToast(translationKey);
            }
          });
        }
        else {
          showErrorToast('AN_UNKNOWN_ERROR_OCCURED');
        }
      }

      return throwError(() => error);
    })
  );

  function showErrorToast(translationKey: string) {
    const message = translate.instant('ERROR.' + translationKey);
    messageService.add({severity: 'error', summary: message, life: 4000});
  }
};