import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/Auth/auth.service';
import { CommonModule } from '@angular/common';
import { CuentasService } from '../../services/Cuentas/cuentas.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatGridListModule,
    MatIconModule
  ]
})
export class DashboardComponent implements OnInit {
  cuentas: any[] = [];

  constructor(
    private http: HttpClient,
    private router: Router,
    private authService: AuthService,
    private cuentasService: CuentasService
  ) {}

  ngOnInit() {
    const userId = this.authService.getUserIdFromToken();
    console.log(userId);
    if(userId != null)
    {
      this.cuentasService.getCuentasUsuario(userId).subscribe({
        next: (response) => {
          this.cuentas = response;
        },
        error: (err) => {
          console.error('Error al obtener las cuentas', err);
        }
      });
    }
  }

  verMovimientos(cuentaId: number) {
    this.router.navigate([`/movimientos/${cuentaId}`]);
  }
}
