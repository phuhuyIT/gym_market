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
  studentId: string;
}
