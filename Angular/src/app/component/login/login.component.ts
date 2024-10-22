import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

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

  router = inject(Router);
  http = inject(HttpClient);

  onLogin() {
    debugger;
    this.http
      .post('https://localhost:7250/api/User/login', this.loginObj)
      .subscribe(
        (res: any) => {
          debugger;
          if (res.success) {
            alert('Login Success');
            sessionStorage.setItem('jwtToken', res.data);
            this.router.navigateByUrl('layout/todo');
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
