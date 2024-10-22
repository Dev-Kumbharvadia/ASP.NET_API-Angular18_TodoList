import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = sessionStorage.getItem("jwtToken");
  const ignoredUrls = [
    'https://localhost:7250/api/User/register',
    'https://localhost:7250/api/User/login'
  ];

  // Check if the request URL is in the ignoredUrls list
  const shouldIgnore = ignoredUrls.some(url => req.url.includes(url));
  debugger;
  console.log(req);
  if (shouldIgnore) {
    // If the request URL is in the ignore list, pass the request without adding the Authorization header
    return next(req);
  }

  const clonedReq = req.clone({
    setHeaders:{
      Authorization: `Bearer ${token}`
    }
  })
  debugger;
  return next(clonedReq);
};
