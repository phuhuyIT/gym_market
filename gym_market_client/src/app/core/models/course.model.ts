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
  courseRatingId: number;
  courseId: string;
  studentId: string;
  ratingValue: number;
  comment: string;
  ratingDate: string;
  studentName?: string;
}

export interface CourseRatingCreateDto {
  courseId: string;
  studentId: string;
  ratingValue: number;
  comment: string;
}

export interface CourseOption {
  optionId: string;
  optionName: string;
  description: string;
  price: number;
  courseId?: string;
}
