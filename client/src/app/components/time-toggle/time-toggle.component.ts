import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-time-toggle',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  template: `
    <button mat-button color="primary" (click)="toggle.emit()">
      {{ showSunrise ? '🌅 Sunrise' : '🌇 Sunset' }}
    </button>
  `,
})
export class TimeToggleComponent {
  @Input() showSunrise: boolean = true;
  @Output() toggle = new EventEmitter<void>();
}
