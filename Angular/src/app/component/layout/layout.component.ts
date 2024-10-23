import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'] // Corrected typo: `styleUrls` should be an array
})
export class LayoutComponent {
  router = inject(Router);
  http = inject(HttpClient);
  loginService = inject(LoginService);

  onLogOff() {
    console.log(this.loginService.USER_ID);
  
    const userId = this.loginService.USER_ID;
  
    // Include the userId in the URL as expected by the backend
    this.http.post(`https://localhost:7250/api/audit/logout/${userId}`, {}).subscribe(
      (res: any) => {
        console.log('Audit log successful', res);
        this.router.navigateByUrl('/login');
        sessionStorage.removeItem('jwtToken');
      },
      (error: any) => {
        console.error('Error logging out:', error);
        if (error.status === 404) {
          console.error('API endpoint not found. Please check the URL.');
        } else if (error.status === 500) {
          console.error('Server error. Please try again later.');
        } else {
          console.error(`Unexpected error: ${error.message}`);
        }
      }
    );
  }
}
