import { Component, OnInit } from '@angular/core';
import { WeatherResponse } from '../../models/weather.model';
import { WeatherService } from '../../services/weather.service';
import { CommonModule } from '@angular/common';
import { TimeToggleComponent } from '../time-toggle/time-toggle.component';
import { UnitToggleComponent } from '../unit-toggle/unit-toggle.component';
import { WeatherCardComponent } from '../weather-card/weather-card.component';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    WeatherCardComponent,
    UnitToggleComponent,
    TimeToggleComponent,
  ],
  templateUrl: 'dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  weatherData: WeatherResponse[] = [];
  trends: Record<string, string> = {};
  unit: string = 'metric';
  showSunrise = true;

  constructor(private weatherService: WeatherService) {}

  trackByName(index: number, weather: WeatherResponse) {
    return weather.name;
  }

  ngOnInit() {
    const savedUnit = sessionStorage.getItem('unit');
    if (savedUnit) this.unit = savedUnit;

    this.loadData();
  }

  loadData() {
    this.weatherService
      .getWeather(this.unit)
      .subscribe((data) => (this.weatherData = data));

    this.weatherService.getTrends().subscribe((data) => (this.trends = data));
  }

  toggleUnit(unit: string) {
    this.unit = unit;
    sessionStorage.setItem('unit', unit);
    this.weatherService
      .setUnitPreference(unit)
      .subscribe(() => this.loadData());
  }

  toggleTimeView() {
    this.showSunrise = !this.showSunrise;
  }
}
