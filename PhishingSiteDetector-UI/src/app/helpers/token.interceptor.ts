// import { AccountService } from './../services/account.service';
// import {
//   HttpErrorResponse,
//   HttpEvent,
//   HttpHandler,
//   HttpInterceptor,
//   HttpRequest,
// } from '@angular/common/http';
// import { Injectable } from '@angular/core';
// import { catchError, Observable, throwError } from 'rxjs';

// @Injectable()
// export class TokenInterceptor implements HttpInterceptor {
//   constructor(public accountService: AccountService) {}

//   intercept(
//     request: HttpRequest<any>,
//     next: HttpHandler
//   ): Observable<HttpEvent<any>> {
//     if (this.accountService.isLoggedIn()) {
//       request = request.clone({
//         setHeaders: {
//           Authorization: `Bearer ${this.accountService.getToken()}`,
//         },
//       });
//     }

//     return next.handle(request).pipe(
//       catchError((error: HttpErrorResponse) => {
//         if (error.status === 401) {
//           this.accountService.logout();
//         }
//         return throwError(() => error);
//       })
//     );
//   }
// }