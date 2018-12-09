import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // all api related request implemented here... some are being called from resolvers
  // apiUrl is defined in App Environment file
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // NOTE: default obsevable is only body and of type 'T' but we can get headers using 'observe'
  // we can send Parameters along with http query string
  // seprate body and headers using pipe(map=>{}) that allows us to use RxJx operators
  GetUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams(); // for sending parameters in http query
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        // check pagination header
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  // single user info presented when 'user' icon on image gets clicked
  GetUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id,);
  }

  // service for updating user profile
  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto (userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

}
