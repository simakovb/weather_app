import {
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  CUSTOM_ELEMENTS_SCHEMA,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { WeatherResponse } from '../../models/weather.model';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-weather-card',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: 'weather-card.component.html',
  styleUrls: ['./weather-card.component.scss'],
  animations: [
    trigger('unitChange', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-4px)' }),
        animate(
          '400ms ease-out',
          style({ opacity: 1, transform: 'translateY(0)' })
        ),
      ]),
      transition(':leave', [
        animate(
          '150ms ease-in',
          style({ opacity: 0, transform: 'translateY(4px)' })
        ),
      ]),
    ]),
    trigger('sunTimeChange', [
      transition('sunrise => sunset', [
        style({ opacity: 0, transform: 'translateX(6px)' }),
        animate(
          '250ms ease-out',
          style({ opacity: 1, transform: 'translateX(0)' })
        ),
      ]),
      transition('sunset => sunrise', [
        style({ opacity: 0, transform: 'translateX(-6px)' }),
        animate(
          '250ms ease-out',
          style({ opacity: 1, transform: 'translateX(0)' })
        ),
      ]),
    ]),
    trigger('fadeInCard', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.95)' }),
        animate('600ms ease-out', style({ opacity: 1, transform: 'scale(1)' })),
      ]),
    ]),
  ],
})
export class WeatherCardComponent implements OnChanges {
  @Input() weather!: WeatherResponse;
  @Input() trend!: string;
  @Input() showSunrise: boolean = true;
  @Input() unit: string = 'metric';

  currentUnitSymbol: string = '';
  previousUnit: string = '';
  weatherCategory: string = 'clear';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['unit']) {
      this.previousUnit = changes['unit'].previousValue;
      this.currentUnitSymbol = this.getTempSymbol();
    }
  }

  getTrendIcon(): string {
    if (this.trend?.includes('rising')) {
      return '⬆️';
    } else if (this.trend?.includes('falling')) {
      return '⬇️';
    } else if (this.trend?.includes('stable')) {
      return '➡️';
    } else {
      return '❔';
    }
  }

  getTempSymbol(): string {
    return this.unit === 'metric' ? '°C' : '°F';
  }

  formatUnixTime(unix: number): string {
    return new Date(unix * 1000).toLocaleTimeString();
  }
}
