export interface Lecture {
  lectureId: string;
  courseId: string;
  moduleId?: string | null;
  moduleTitle?: string | null;
  moduleOrder?: number | null;
  title: string;
  description?: string | null;
  activityType?: string | null;
  order: number;
  duration: number;
  prerequisiteLectureId?: string | null;
  unlockAfterDays?: number | null;
  availableFrom?: string | null;
  availableUntil?: string | null;
  isPreview?: boolean;
  isPublished?: boolean;
  isLocked?: boolean;
  lockReason?: string | null;
  unlocksAt?: string | null;
}

export interface LectureMaterial {
  materialId: string;
  lectureId: string;
  materialType: string;
  materialContent: string;
}

export interface CourseModule {
  moduleId: string;
  courseId: string;
  title: string;
  description?: string | null;
  order?: number | null;
  prerequisiteModuleId?: string | null;
  unlockAfterDays?: number | null;
  availableFrom?: string | null;
  availableUntil?: string | null;
  isPublished: boolean;
  createdAt?: string;
  updatedAt?: string;
}
