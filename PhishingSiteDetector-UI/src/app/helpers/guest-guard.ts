import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AccountService } from '../services/common/account-service';

@Injectable({ providedIn: 'root' })
export class GuestGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) {}

  canActivate(): boolean {
    if (this.accountService.isAuthenticated()) {
      this.router.navigate(['']);
      return false;
    }
    return true;
  }
}