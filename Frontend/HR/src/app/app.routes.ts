import { Routes } from '@angular/router';
import { Employees } from './components/employees/employees';
import { Departments } from './components/departments/departments';
import { Login } from './components/login/login';
import { authGuard } from './auth-guard/auth-guard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/employees',
        pathMatch: 'full'
    },
    {
        path: 'employees',
        component: Employees,
        canActivate: [authGuard]
    },
    {
        path: 'departments',
        component: Departments,
        canActivate: [authGuard]
    },
    {
        path: 'login',
        component: Login
    }
];
