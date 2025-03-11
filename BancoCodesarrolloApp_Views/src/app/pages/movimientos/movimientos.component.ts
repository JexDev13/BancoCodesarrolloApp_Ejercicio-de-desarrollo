import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CuentasService } from '../../services/Cuentas/cuentas.service';
import { MovimientosService } from '../../services/Movimientos/movimientos.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MovimientoCreacionDTO } from '../../models/movimiento.model';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-movimientos',
  templateUrl: './movimientos.component.html',
  styleUrls: ['./movimientos.component.css'],
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatGridListModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatOptionModule,
    ReactiveFormsModule,
    MatTableModule,
    MatSelectModule
  ]
})
export class MovimientosComponent implements OnInit {
  movimientos: any[] = [];
  numeroCuenta: string = '';
  movimientoForm: FormGroup;

  displayedColumns: string[] = ['fecha', 'tipoMovimiento', 'valor', 'saldo'];

  constructor(
    private route: ActivatedRoute,
    private cuentasService: CuentasService,
    private movimientosService: MovimientosService,
    private fb: FormBuilder
  ) {
    this.movimientoForm = this.fb.group({
      tipoMovimiento: ['', Validators.required],
      valor: [null, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.numeroCuenta = this.route.snapshot.paramMap.get('id')!;
    this.getMovimientos();
  }

  getMovimientos(): void {
    this.movimientosService.getMovimientosPorCuenta(this.numeroCuenta).subscribe({
      next: (response) => {
        this.movimientos = response;
      },
      error: (err) => {
        console.error('Error al obtener los movimientos', err);
      }
    });
  }

  crearMovimiento(): void {
    if (this.movimientoForm.invalid) {
      return;
    }

    const nuevoMovimiento: MovimientoCreacionDTO = {
      cuentaId: parseInt(this.numeroCuenta, 10),
      tipoMovimiento: this.movimientoForm.value.tipoMovimiento,
      valor: this.movimientoForm.value.valor
    };

    if (
      (nuevoMovimiento.tipoMovimiento === 'debito' && nuevoMovimiento.valor <= 0) ||
      (nuevoMovimiento.tipoMovimiento === 'credito' && nuevoMovimiento.valor >= 0)
    ) {
      alert('Valor inválido para este tipo de movimiento.');
      return;
    }

    this.movimientosService.crearMovimiento(nuevoMovimiento).subscribe({
      next: (response) => {
        this.getMovimientos();
        this.movimientoForm.reset();
      },
      error: (err) => {
        console.error('Error al crear el movimiento', err);
      }
    });
  }


  validarValor() {
    const tipoMovimiento = this.movimientoForm.get('tipoMovimiento')?.value;
    const valor = this.movimientoForm.get('valor')?.value;

    if (tipoMovimiento === 'credito' && valor > 3000) {
      alert('El valor no puede ser mayor a 3000 para un movimiento de crédito.');
      this.movimientoForm.get('valor')?.setValue(null);
    } else if (valor < 0) {
      alert('El valor no puede ser menor a 0.');
      this.movimientoForm.get('valor')?.setValue(null);
    }
  }

}
