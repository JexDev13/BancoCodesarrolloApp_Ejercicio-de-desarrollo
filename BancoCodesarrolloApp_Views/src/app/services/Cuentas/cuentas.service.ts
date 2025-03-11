import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CuentasService {

  private apiUrl = 'https://localhost:7213/api/Cuentas/usuario';

  constructor(private http: HttpClient) { }

  getCuentasUsuario(userId: number): Observable<any[]> {
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<any[]>(`${this.apiUrl}/${userId}`, { headers });
  }
}
