import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  let token = localStorage.getItem('token'); // get token from local storage
  if (token != null) { // check if token not null
    let authReq = req.clone({ // create new request cloned from old request
      headers: req.headers.set("Authorization", "Bearer " + token) // add token header to new request
    });

    return next(authReq); // return new request with token header
  }

  return next(req); // return old request without token
};
