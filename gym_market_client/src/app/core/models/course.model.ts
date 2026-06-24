export interface Course {
  courseId: string;
  trainerId: string;
  title: string;
  description: string;
  type: string;
  category: string;
  price: number;
  additionalPrice: number;
  startDate: string;
  endDate: string;
  duration: number;
  maxParticipants: number;
  status?: 'Draft' | 'Published' | 'Archived';
  rating: number;
  getFileDtos: CourseFile[];
  statusPayment?: string;
}

export interface CourseFile {
  fileId: string;
  courseId: string;
  url: string;
  typeFile: 'IMAGE' | 'VIDEO';
}

export interface CourseRating {
  ratingId: string;
  courseId: string;
  studentId: string;
  ratingValue: number;
  reviewComment: string;
}

export interface CourseRatingCreateDto {
  courseId: string;
  ratingValue: number;
  reviewComment: string;
}

export interface CourseOption {
  optionId: string;
  courseId: string;
  optionName: string;
  description: string;
  price: number;
}
