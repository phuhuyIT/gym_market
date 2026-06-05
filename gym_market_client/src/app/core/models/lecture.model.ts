export interface Lecture {
  lectureId: string;
  courseId: string;
  title: string;
  order: number;
  duration: number;
}

export interface LectureMaterial {
  materialId: string;
  lectureId: string;
  materialType: string;
  materialContent: string;
}
