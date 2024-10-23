import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginObj: any = {
    username: '',
    password: '',
  };
  
  loginServices = inject(LoginService)

  router = inject(Router);
  http = inject(HttpClient);

  onLogin() {
    this.http
      .post('https://localhost:7250/api/User/login', this.loginObj)
      .subscribe(
        (res: any) => {
          if (res.success) {
            alert('Login Success');
            sessionStorage.setItem('jwtToken', res.data);
            this.router.navigateByUrl('layout/todo');
            this.loginServices.USER_ID = res.message;
          } else {
            alert('Error occurred');
          }
        },
        (error) => {
          console.error('Error:', error); // Log the error to troubleshoot
          alert('Error in login request');
        }
      );
  }


}
