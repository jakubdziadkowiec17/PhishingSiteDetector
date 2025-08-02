import { inject, Injectable} from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { Role } from '../constants/role';
import { Claim } from '../constants/claim';
import { AccountService } from '../services/common/account-service';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const accessToken = this.accountService.getAccessToken();
    if (!accessToken) return this.redirectToLogin(state.url);

    try {
        const decoded = jwtDecode<{ [Claim.Roles]: string[] }>(accessToken);
        const roles = decoded[Claim.Roles];
        if (roles?.includes(Role.Admin)) return true;
    }
    catch {
        this.accountService.deleteTokens();
        return this.redirectToLogin(state.url);
    }
    
    return this.redirectToLogin(state.url);
  }

  private redirectToLogin(redirectUrl: string): boolean {
    this.router.navigate(['/login'], { queryParams: { redirect: redirectUrl } });
    return false;
  }
}