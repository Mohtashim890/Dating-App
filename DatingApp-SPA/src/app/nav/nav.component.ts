import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  // make object to hold login form data
  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    this.authService.Login(this.model).subscribe(next => {
      this.alertify.success('logged in succesfully');
    }, error => {
      this.alertify.error(error);
    });
  }

  loggedIn() {
    return this.authService.LoggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertify.message('logged out succesfully');
  }

}
