import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChartDTO } from '../../interfaces/chart-dto';
import { Server } from '../../constants/server';

@Injectable({
  providedIn: 'root'
})
export class SiteLogService {
  private readonly baseUrl = Server.apiUrl + 'site-log';

  constructor(private http: HttpClient) {}

  getSiteLogs(year: number): Observable<ChartDTO[]> {
    const params = new HttpParams().set('year', year.toString());
    return this.http.get<ChartDTO[]>(this.baseUrl, { params });
  }
}