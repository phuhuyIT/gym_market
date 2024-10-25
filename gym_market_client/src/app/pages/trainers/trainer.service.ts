import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Trainer } from './trainer.model';
@Injectable({
    providedIn: 'root'
  })
  export class TrainerService {
  
    // Cập nhật URL của API backend
    private baseUrl = 'https://localhost:7115/api/Trainer'; 
  
    constructor(private http: HttpClient) { }
  
    // Phương thức lấy danh sách tất cả trainers
    getAllTrainers(): Observable<Trainer[]> {
        return this.http.get<Trainer[]>(`${this.baseUrl}/get-lists`);
      }
    
  }