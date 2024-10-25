import { Component, OnInit } from '@angular/core';
import { TrainerService } from './trainer.service'; // Đảm bảo đường dẫn đúng
import { Trainer } from './trainer.model'; // Đảm bảo bạn đã định nghĩa model Trainer

@Component({
  selector: 'app-root',
  templateUrl: './trainers.component.html',
  styleUrls: ['./trainers.component.css']
})
export class AppComponent implements OnInit {
  trainers: Trainer[] = []; // Khai báo biến để lưu danh sách trainers

  constructor(private trainerService: TrainerService) { }

  ngOnInit(): void {
    this.fetchAllTrainers(); // Gọi hàm để fetch dữ liệu khi component khởi tạo
  }

  fetchAllTrainers(): void {
    this.trainerService.getAllTrainers().subscribe({
      next: (data: Trainer[]) => {
        this.trainers = data; // Lưu dữ liệu vào biến trainers
      },
      error: (error) => {
        console.error('Error fetching trainers:', error); // Xử lý lỗi
      },
      complete: () => {
        console.log('Fetch complete'); // (Tùy chọn) Thực hiện khi hoàn thành
      }
    });
  }
}
