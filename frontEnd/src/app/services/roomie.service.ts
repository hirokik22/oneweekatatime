import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Roomie } from '../model/roomie';

@Injectable({
  providedIn: 'root',
})
export class RoomieService {
  private baseUrl: string = 'http://localhost:5193/api/Roomie';

  constructor(private http: HttpClient) {}

  getRoomiesByLoginId(loginId: number): Observable<Roomie[]> {
    return this.http.get<Roomie[]>(`${this.baseUrl}/${loginId}`);
  }
}