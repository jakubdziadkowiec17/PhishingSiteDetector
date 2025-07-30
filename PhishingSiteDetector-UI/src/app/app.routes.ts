import { Routes } from '@angular/router';
import { AdminGuard } from './helpers/admin-guard';
import { HomeComponent } from './components/home/home';
import { LoginComponent } from './components/login/login';
import { SettingsComponent } from './components/settings/settings';
import { StatisticsComponent } from './components/statistics/statistics';
import { DataSetsComponent } from './components/data-sets/data-sets';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'settings',
        component: SettingsComponent,
        canActivate: [AdminGuard]
    },
    {
        path: 'statistics',
        component: StatisticsComponent,
        canActivate: [AdminGuard]
    },
    {
        path: 'data-sets',
        component: DataSetsComponent,
        canActivate: [AdminGuard]
    },
    {
        path: '**',
        redirectTo: ''
    }
];
