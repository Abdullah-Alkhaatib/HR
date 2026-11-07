import { Routes } from '@angular/router';
import { Employees } from './components/employees/employees';
import { Departments } from './components/departments/departments';
import { Login } from './components/login/login';

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/employees',
        pathMatch: 'full'
    },
    {
        path: 'employees',
        component: Employees
    },
    {
        path: 'departments',
        component: Departments
    },
    {
        path: 'login',
        component: Login
    }
];
