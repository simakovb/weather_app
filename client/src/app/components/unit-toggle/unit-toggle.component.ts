import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonToggleModule } from '@angular/material/button-toggle';

@Component({
  selector: 'app-unit-toggle',
  standalone: true,
  imports: [CommonModule, MatButtonToggleModule],
  template: `
    <mat-button-toggle-group [value]="unit" (change)="onChange($event.value)">
      <mat-button-toggle value="metric">°C</mat-button-toggle>
      <mat-button-toggle value="imperial">°F</mat-button-toggle>
    </mat-button-toggle-group>
  `,
  styles: [
    `
      :host {
        display: inline-block;
      }
    `,
  ],
})
export class UnitToggleComponent {
  @Input() unit: string = 'metric';
  @Output() unitChange = new EventEmitter<string>();

  onChange(unit: string) {
    this.unitChange.emit(unit);
  }
}
