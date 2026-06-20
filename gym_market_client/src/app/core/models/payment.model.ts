export interface Payment {
  paymentId: string;
  courseId: string;
  studentId: string;
  paymentAmount: number;
  paymentDate: string;
  paymentStatus: 'Pending' | 'Paid' | 'Canceled';
  paymentType: string;
  note: string;
  fullName?: string;
  // AppUser id of the student — used to open a 1:1 chat (distinct from studentId).
  userId?: string;
  createdAt: string;
}

export interface CancelPaymentDto {
  paymentId: string;
  note: string;
}
