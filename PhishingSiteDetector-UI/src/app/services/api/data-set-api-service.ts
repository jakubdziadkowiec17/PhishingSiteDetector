import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Server } from '../../constants/server';
import { ListPageDTO } from '../../interfaces/list-page-dto';
import { DataSetItemDTO } from '../../interfaces/data-set-item-dto';
import { ResponseDTO } from '../../interfaces/response-dto';

@Injectable({
  providedIn: 'root'
})
export class DataSetApiService {
  private readonly baseUrl = Server.apiUrl + 'data-set';

  constructor(private http: HttpClient) {}

  upload(dataSet: FormData): Observable<ResponseDTO> {
    return this.http.post<ResponseDTO>(this.baseUrl, dataSet);
  }

  getDataSets(searchText: string = "", pageNumber: number, pageSize: number): Observable<ListPageDTO<DataSetItemDTO>> {
    const params: any = {
      pageNumber,
      pageSize,
      searchText
    };
    return this.http.get<ListPageDTO<DataSetItemDTO>>(this.baseUrl, { params });
  }

  downloadDataSet(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/download/${id}`, {
      responseType: 'blob'
    });
  }

  updateActivity(id: number, dataSetItem: DataSetItemDTO): Observable<ResponseDTO> {
    return this.http.put<ResponseDTO>(`${this.baseUrl}/${id}`, dataSetItem);
  }

  deleteDataSet(id: number): Observable<ResponseDTO> {
    return this.http.delete<ResponseDTO>(`${this.baseUrl}/${id}`);
  }
}