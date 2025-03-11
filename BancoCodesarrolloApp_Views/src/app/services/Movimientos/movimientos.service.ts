import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MovimientoCreacionDTO } from '../../models/movimiento.model';

@Injectable({
  providedIn: 'root'
})
export class MovimientosService {

  private apiUrl = 'https://localhost:7213/api/Movimientos';

  constructor(private http: HttpClient) { }

  getMovimientosPorCuenta(numeroCuenta: string): Observable<any[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<any[]>(`${this.apiUrl}/cuenta/${numeroCuenta}`, { headers });
  }

  crearMovimiento(movimiento: MovimientoCreacionDTO): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.post<any>(`${this.apiUrl}`, movimiento, { headers });
  }
}
