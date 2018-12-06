import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  /*photoUrl is observable property so that other components can subscribe to it...
    it can also act as observer i.e components can change its value*/
  photoUrl = new BehaviorSubject<string>('../../assets/user.jpg');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  /*this function will update the phtoUrl observable */
  changeMemberPhoto (photoUrl: string) {
    this.photoUrl.next(photoUrl); // next() will update BehaviorSubject observable
  }

  Login(model: any) {
    // note that response of this request will be a token [and a user object]
    // use pipe observable from RxJx to handle/store that token locally
    return this.http.post(this.baseUrl + 'login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl); // update photoUrl observable
        }
      })
    );
  }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

  // service to check if user logged in
  LoggedIn() {
    const token = localStorage.getItem('token');
    // if token is expired or absent then isTokenExpired() return true...
    return !this.jwtHelper.isTokenExpired(token);
  }

}
