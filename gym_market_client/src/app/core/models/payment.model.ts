export interface Payment {
  paymentId: string;
  courseId: string;
  studentId: string;
  paymentAmount: number;
  paymentDate: string;
  paymentStatus: 'Pending' | 'Paid' | 'Canceled';
  paymentType: string;
  note: string;
  createdAt: string;
}

export interface CancelPaymentDto {
  paymentId: string;
  note: string;
}
