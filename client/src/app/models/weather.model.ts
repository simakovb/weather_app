export interface WeatherResponse {
  name: string;
  main: {
    temp: number;
  };
  weather: [
    {
      main: any;
      description: string;
      icon: string;
    }
  ];
  sys: {
    sunrise: number;
    sunset: number;
  };
}
