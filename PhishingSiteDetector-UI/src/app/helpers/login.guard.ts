// import { Injectable } from '@angular/core';
// import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';
// import { AccountService } from './../services/account.service';
// import { map, Observable, of } from 'rxjs';
// import { Role } from '../Constants/Role';

// @Injectable({
//   providedIn: 'root',
// })

// export class LoginGuard implements CanActivate {
//   constructor(private accountService: AccountService, private router: Router) {}

//   canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
//     if (!this.accountService.isLoggedIn()) {
//       return of(true);
//     }

//     this.router.navigate(['']);
//     return of(false);
//   }
// }