import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {

  let router = inject(Router); // Create Object using dependency injection without constructor
  let token = localStorage.getItem('token');

  if (token != null) {
    return true;
  }
  else {
    router.navigate(['/login']);
    return false;
  }

};
