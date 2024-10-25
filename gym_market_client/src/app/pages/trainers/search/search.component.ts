import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TrainerService } from '../trainer.service'; // Đảm bảo đường dẫn đúng
import { Trainer } from '../trainer.model'; // Đảm bảo bạn đã định nghĩa model Trainer
import { CommonModule } from '@angular/common'; // Nhập CommonModule

@Component({
    selector: 'app-search',
    standalone: true,
    imports: [CommonModule, RouterLink], // Nhập CommonModule
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss'], // Chỉnh sửa từ styleUrl thành styleUrls
})
export class SearchComponent implements OnInit {
    trainers: Trainer[] = []; // Khai báo biến để lưu danh sách trainers

    constructor(private trainerService: TrainerService) { }

    ngOnInit() {
        this.fetchAllTrainers(); // Gọi hàm để lấy dữ liệu khi component khởi tạo
    }

    fetchAllTrainers(): void {
        this.trainerService.getAllTrainers().subscribe({
            next: (data: Trainer[]) => {
                this.trainers = data; // Lưu dữ liệu vào biến trainers
            },
            error: (error) => {
                console.error('Error fetching trainers:', error); // Xử lý lỗi
            }
        });
    }

    onSubmit() {
        // Xử lý logic khi người dùng gửi form
        console.log('Form submitted!'); // Hoặc thực hiện hành động khác
    }
}
