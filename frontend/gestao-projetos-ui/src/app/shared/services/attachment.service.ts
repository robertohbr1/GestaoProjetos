import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AttachmentResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AttachmentService {
  private apiUrl = 'https://localhost:5001/api/attachments';

  constructor(private http: HttpClient) {}

  upload(issueId: number, file: File): Observable<AttachmentResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<AttachmentResponse>(`${this.apiUrl}/issue/${issueId}`, formData);
  }

  download(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}`, { responseType: 'blob' });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
