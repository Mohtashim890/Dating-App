import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // we can access form using viewChild decorator
  @ViewChild('registerForm') registerForm: NgForm;
  model: any = {};
  // by declaring with @, variable becomes available in other components
  @Output() cancelRegister = new EventEmitter();

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
    this.authService.Register(this.model).subscribe(() => {
      this.alertify.success('Registration successful');
      this.registerForm.reset();
    }, error => {
      this.alertify.error(error);
    });
  }
  
  cancell() {
    this.cancelRegister.emit(false);
  }

}
