import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // all api related request implemented here...are being called from resolvers
  // apiUrl is defined in App Environment file
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // NOTE: default obsevable is only body and of type 'T' but we can get headers using 'observe'
  // we can send Parameters along with http query string
  // seprate body and headers using pipe(map=>{}) that allows us to use RxJx operators
  GetUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    // create an object that can store users in array and paging information as interface
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

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        // check pagination header
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  // single user info presented when 'user' icon on image gets clicked
  GetUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
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

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?): Observable<PaginatedResult<Message[]>> {
    // create an object that can store messages in array and paging information as interface
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams(); // for sending parameters in http query
    params = params.append('messageContainer', messageContainer);
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          // check pagination header
          if (response.headers.get('Pagination') !== null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  getMessageThread (id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage (id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage (id: number, userId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
  }

  markAsRead (userId: number, id: number) {
    // not returning anything so subscribe here...
    // when user clicks on message in Unread..he will redirected to messages tab where loadMessages()
    // called so use this function there to set as read
    this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id + '/read', {})
    .subscribe();
  }
}
