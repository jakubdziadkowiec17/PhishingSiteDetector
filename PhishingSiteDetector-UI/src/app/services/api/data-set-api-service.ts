import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Server } from '../../constants/server';
import { ListPageDTO } from '../../interfaces/list-page-dto';
import { DataSetItemDTO } from '../../interfaces/data-set-item-dto';
import { ResponseDTO } from '../../interfaces/response-dto';
import { DataSetStatusDTO } from '../../interfaces/data-set-status-dto';

@Injectable({
  providedIn: 'root'
})
export class DataSetApiService {
  private readonly baseUrl = Server.apiUrl + 'data-set';

  constructor(private http: HttpClient) {}

  uploadDataSet(dataSet: FormData): Observable<ResponseDTO> {
    return this.http.post<ResponseDTO>(this.baseUrl, dataSet);
  }

  getDataSets(searchText: string = "", pageNumber: number, pageSize: number, sortField?: string, sortOrder?: number): Observable<ListPageDTO<DataSetItemDTO>> {
    const params: any = {
      pageNumber,
      pageSize,
      searchText,
      sortField,
      sortOrder
    };
    return this.http.get<ListPageDTO<DataSetItemDTO>>(this.baseUrl, { params });
  }

  downloadDataSet(id: number): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.baseUrl}/download/${id}`, {
      observe: 'response',
      responseType: 'blob'
    });
  }

  updateActivityForDataSet(id: number, dataSetStatusDTO: DataSetStatusDTO): Observable<ResponseDTO> {
    return this.http.put<ResponseDTO>(`${this.baseUrl}/${id}`, dataSetStatusDTO);
  }

  deleteDataSet(id: number): Observable<ResponseDTO> {
    return this.http.delete<ResponseDTO>(`${this.baseUrl}/${id}`);
  }
}