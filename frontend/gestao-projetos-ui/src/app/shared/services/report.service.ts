import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardSummary, DeveloperWorkload, IssueResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = 'https://localhost:5001/api/reports';

  constructor(private http: HttpClient) {}

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.apiUrl}/summary`);
  }

  getCompleted(startDate: string, endDate: string): Observable<IssueResponse[]> {
    let params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<IssueResponse[]>(`${this.apiUrl}/completed`, { params });
  }

  getWorkload(): Observable<DeveloperWorkload[]> {
    return this.http.get<DeveloperWorkload[]>(`${this.apiUrl}/workload`);
  }

  getPending(): Observable<IssueResponse[]> {
    return this.http.get<IssueResponse[]>(`${this.apiUrl}/pending`);
  }
}
