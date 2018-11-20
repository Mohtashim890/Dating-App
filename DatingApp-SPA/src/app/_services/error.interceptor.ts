import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable ()

export class ErrorInterceptor implements HttpInterceptor
{
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error instanceof HttpErrorResponse) {
                    // handle Authorization error
                    if(error.status == 401) {
                        return throwError(error.statusText);
                    }
                    // handling Application-Error
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        return throwError(applicationError);
                    }
                    // handling Server Errors
                    const serverError = error.error;
                    let modalStateError = '';
                    if (serverError && typeof serverError === 'object') {
                        for (const key in serverError) {
                            if(serverError[key]) {
                                modalStateError += serverError[key] + '\n';
                            }
                        }
                    }
                    // throw error if there is any...
                    return throwError(modalStateError || serverError || 'Server Error');
                }
            })
        );
    }
}

// add this interceptor to angular's array of Http Interceptor using 'provide'
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true  // to avoid replacing whole array
}
