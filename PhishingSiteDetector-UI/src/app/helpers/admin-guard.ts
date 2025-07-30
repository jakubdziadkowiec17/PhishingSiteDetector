import { inject} from '@angular/core';
import { CanActivateFn} from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { CookieService } from 'ngx-cookie-service';
import { Role } from '../constants/role';

export const AdminGuard: CanActivateFn = (route, state) => {
    const cookieService = inject(CookieService);
    const accessToken = cookieService.get('AccessToken');
    if (!accessToken) return redirectToLogin(state.url);

    try {
        const decoded: any = jwtDecode(accessToken);
        if (decoded?.role === Role.Admin) {
            return true;
        }
    }
    catch (e) {
        cookieService.delete('AccessToken');
        cookieService.delete('RefreshToken');
    }

    return redirectToLogin(state.url);
};

function redirectToLogin(redirectUrl: string) {
    window.location.href = `/login?redirect=${encodeURIComponent(redirectUrl)}`;
    return false;
}