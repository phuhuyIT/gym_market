export interface Trainer {
    trainerId: string;           // ID của huấn luyện viên
    name: string;                // Tên huấn luyện viên
    email?: string | null;       // Email (tùy chọn)
    password?: string | null;    // Mật khẩu (tùy chọn)
    certification: string;       // Chứng chỉ
    bio?: string | null;         // Tiểu sử (tùy chọn)
    experience?: number | null;  // Kinh nghiệm (tùy chọn)
    rating: number;              // Đánh giá
    profilePicture?: string | null; // Hình ảnh đại diện (tùy chọn)
    createdAt: string;           // Ngày tạo
    updatedAt?: string | null;
    image?: string;   
    userId?: string | null;      // ID người dùng (tùy chọn)
    courses: any[];              // Danh sách khóa học (tùy chọn, có thể định nghĩa rõ hơn)
    messages: any[];             // Danh sách tin nhắn (tùy chọn, có thể định nghĩa rõ hơn)
    appUser?: any | null;        // Người dùng ứng dụng (tùy chọn, có thể định nghĩa rõ hơn)
  }