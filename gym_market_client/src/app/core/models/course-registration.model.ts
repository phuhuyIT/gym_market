import { Course } from "./course.model";

export interface CourseRegistration {
  courseRegistrationId: number;
  courseId: string;
  studentId: string;
  registrationDate: string;
  status: string;
  course?: Course;
}

export interface RegisterCourseDto {
  courseId: string;
  optionIds?: string[];
}

export interface CoursePaymentOption {
  optionId: string;
  optionName?: string;
  price: number;
}

// Bank-transfer payment details for one registered course (see backend
// CoursePaymentInfoDto). Used by the course-payment screen to build a VietQR.
export interface CoursePaymentInfo {
  paymentId?: string;
  courseId: string;
  courseTitle?: string;
  amount: number;
  courseAmount: number;
  optionsAmount: number;
  options: CoursePaymentOption[];
  status?: string;
  reference?: string;
  bankBin?: string;
  bankAccountNo?: string;
  bankAccountName?: string;
  trainerName?: string;
  bankConfigured: boolean;
}
