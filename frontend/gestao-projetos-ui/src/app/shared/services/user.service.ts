import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserResponse } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5151/api/users';

  constructor(private http: HttpClient) {}

  getAll(): Observable<UserResponse[]> {
    return this.http.get<UserResponse[]>(this.apiUrl);
  }
}
