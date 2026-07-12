import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TimeLogResponse, TimeLogRequest } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class TimesheetService {
  private apiUrl = 'https://localhost:5001/api/timelogs';

  constructor(private http: HttpClient) {}

  getByIssue(issueId: number): Observable<TimeLogResponse[]> {
    return this.http.get<TimeLogResponse[]>(`${this.apiUrl}/issue/${issueId}`);
  }

  create(log: TimeLogRequest): Observable<TimeLogResponse> {
    return this.http.post<TimeLogResponse>(this.apiUrl, log);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
