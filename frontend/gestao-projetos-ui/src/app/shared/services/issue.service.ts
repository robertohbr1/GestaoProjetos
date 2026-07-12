import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IssueResponse, IssueDetailResponse, IssueRequest } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class IssueService {
  private apiUrl = 'http://localhost:5151/api/issues';

  constructor(private http: HttpClient) {}

  getFiltered(filters: {
    implementationType?: string;
    projectId?: number;
    status?: string;
    assignedToUserId?: number;
    priority?: string;
    searchTerm?: string;
  }): Observable<IssueResponse[]> {
    let params = new HttpParams();
    
    if (filters.implementationType) {
      params = params.set('implementationType', filters.implementationType);
    }
    if (filters.projectId) {
      params = params.set('projectId', filters.projectId.toString());
    }
    if (filters.status) {
      params = params.set('status', filters.status);
    }
    if (filters.assignedToUserId) {
      params = params.set('assignedToUserId', filters.assignedToUserId.toString());
    }
    if (filters.priority) {
      params = params.set('priority', filters.priority);
    }
    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }

    return this.http.get<IssueResponse[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<IssueDetailResponse> {
    return this.http.get<IssueDetailResponse>(`${this.apiUrl}/${id}`);
  }

  create(issue: IssueRequest): Observable<IssueResponse> {
    return this.http.post<IssueResponse>(this.apiUrl, issue);
  }

  update(id: number, issue: IssueRequest): Observable<IssueResponse> {
    return this.http.put<IssueResponse>(`${this.apiUrl}/${id}`, issue);
  }

  updatePriority(id: number, priority: string): Observable<IssueResponse> {
    return this.http.patch<IssueResponse>(`${this.apiUrl}/${id}/priority`, { priority });
  }

  updateStatus(id: number, status: string, assignedToUserId?: number): Observable<IssueResponse> {
    return this.http.patch<IssueResponse>(`${this.apiUrl}/${id}/status`, { status, assignedToUserId });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
