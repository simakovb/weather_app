import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WeatherResponse } from '../models/weather.model';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  private apiUrl = 'http://localhost:5000';

  constructor(private http: HttpClient) {}

  getWeather(unit: string): Observable<WeatherResponse[]> {
    return this.http.get<WeatherResponse[]>(
      `${this.apiUrl}/weather?unit=${unit}`
    );
  }

  getTrends(): Observable<Record<string, string>> {
    return this.http.get<Record<string, string>>(
      `${this.apiUrl}/weather/trend`
    );
  }

  getForecast(): Observable<Record<string, string>> {
    return this.http.get<Record<string, string>>(
      `${this.apiUrl}/weather/forecast`
    );
  }

  setUnitPreference(unit: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/unit`, { unit });
  }

  getUnitPreference(): Observable<any> {
    return this.http.get(`${this.apiUrl}/unit`);
  }
}
