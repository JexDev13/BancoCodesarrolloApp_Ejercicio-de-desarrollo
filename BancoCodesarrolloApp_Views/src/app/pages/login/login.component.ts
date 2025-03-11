import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { AuthService } from '../../services/Auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatCardModule,
    FormsModule
  ]
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.loading = true;

    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
        this.authService.saveSession(response);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        alert(err.error || 'Usuario o contraseña incorrectos.');
        this.loading = false;
      }
    });
  }
}
