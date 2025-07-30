import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UrlDTO } from '../../interfaces/url-dto';
import { UrlPredictionDTO } from '../../interfaces/url-prediction-dto';
import { Server } from '../../constants/server';

@Injectable({
  providedIn: 'root'
})
export class UrlPredictionService {
  private readonly baseUrl = Server.apiUrl + 'url-prediction';

  constructor(private http: HttpClient) {}

  predict(urlDto: UrlDTO): Observable<UrlPredictionDTO> {
    return this.http.post<UrlPredictionDTO>(this.baseUrl, urlDto);
  }
}