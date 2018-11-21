import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;

  constructor(private http: HttpClient) { }

  Login(model: any) {
    // note that response of this request will be a token
    // use pipe observable from RxJx to handle/store that token locally
    return this.http.post(this.baseUrl + 'login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          console.log(this.decodedToken);
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
