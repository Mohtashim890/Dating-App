import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  // by declaring with @, variable becomes available in other components
  @Output() cancelRegister = new EventEmitter();

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.authService.Register(this.model).subscribe(() => {
      console.log('Registration successful');
    }, error => {
      console.log('error');
    });
  }

  cancell() {
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
