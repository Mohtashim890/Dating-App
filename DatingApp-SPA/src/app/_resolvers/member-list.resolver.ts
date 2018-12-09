import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 5; // items per page
    constructor(private userService: UserService, private router: Router,
        private alertify: AlertifyService) {}

    // Resolve functions automatically subscribes to service methods
    // returning Headers+Body (due to pagination)
    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.GetUsers(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
