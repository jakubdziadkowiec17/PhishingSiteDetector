// import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
// import { AccountService } from '../services/account.service';
// import { map, Observable, of } from 'rxjs';
// import { Injectable } from '@angular/core';
// import { Role } from '../Constants/Role';

// @Injectable({
//   providedIn: 'root',
// })

// export class AdminGuard implements CanActivate {
//   constructor(private accountService: AccountService, private router: Router) {}

//   canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
//     return this.accountService.hasAccessToRoles([Role.Admin]).pipe(
//       map(hasAccess => {
//         if (hasAccess) {
//           return true;
//         } 
//         else {
//           this.router.navigate(['']);
//           return false;
//         }
//       })
//     );
//   }
// }
