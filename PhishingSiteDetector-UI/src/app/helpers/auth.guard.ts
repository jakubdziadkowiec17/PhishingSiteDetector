// import { Injectable } from '@angular/core';
// import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';
// import { AccountService } from './../services/account.service';
// import { map, Observable, of } from 'rxjs';
// import { Role } from '../Constants/Role';

// @Injectable({
//   providedIn: 'root',
// })

// export class AuthGuard implements CanActivate {
//   constructor(private accountService: AccountService) {}

//   canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
//     if (!this.accountService.isLoggedIn()) {
//       this.accountService.logout();
//       return of(false);
//     }

//     return this.accountService.hasAccessToRoles([Role.Admin, Role.Employee]).pipe(
//       map(hasAccess => {
//         if (hasAccess) {
//           return true;
//         } else {
//           this.accountService.logout();
//           return false;
//         }
//       })
//     );
//   }
// }