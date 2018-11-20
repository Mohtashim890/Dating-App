import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';

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
        }
      })
    );
  }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

}
