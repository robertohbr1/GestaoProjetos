import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CommentResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = 'http://localhost:5151/api/comments';

  constructor(private http: HttpClient) {}

  create(issueId: number, content: string): Observable<CommentResponse> {
    return this.http.post<CommentResponse>(`${this.apiUrl}/issue/${issueId}`, { content });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
