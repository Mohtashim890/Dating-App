import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm, FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // we can access form using viewChild decorator
  // @ViewChild('registerForm') registerForm: NgForm;
  user: User;
  // for changing theme of datepicker. Note: Partial<> makes all class properties optional
  bsConfig: Partial<BsDatepickerConfig>;
  // reactive register form
  registerForm: FormGroup;
  // by declaring with @, variable becomes available in other components
  @Output() cancelRegister = new EventEmitter();

  constructor(private authService: AuthService, private alertify: AlertifyService,
              private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
  }

  // create register form
  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  // custom validator for FormGroup
  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }
  register() {
    if (this.registerForm.valid) {
      // copy values from registerForm to user object
      // make clone of registerForm values by using empty object
      this.user = Object.assign({}, this.registerForm.value);

      this.authService.Register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
        this.registerForm.reset();
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.authService.Login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });
    }
  }

  cancell() {
    this.cancelRegister.emit(false);
  }

}
