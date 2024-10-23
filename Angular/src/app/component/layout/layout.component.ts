import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterLink, RouterOutlet],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'] // Corrected typo: `styleUrls` should be an array
})

export class LayoutComponent {
  authService = inject(AuthService);
  router = inject(Router);

  onLogOff() {
    // Call the logout method from AuthService
    this.authService.logout(); // No need to pass userId, logout handles it internally
  }

  
}
