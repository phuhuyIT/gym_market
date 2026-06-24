export interface Payment {
  paymentId: string;
  courseId: string;
  studentId: string;
  paymentAmount: number;
  paymentDate: string;
  paymentStatus: 'Pending' | 'Paid' | 'Canceled' | 'Not Started' | 'Expired';
  paymentType: string;
  note: string;
  fullName?: string;
  courseTitle?: string;
  // AppUser id of the student — used to open a 1:1 chat (distinct from studentId).
  userId?: string;
  canApprove?: boolean;
  canCancel?: boolean;
  actionBlockedReason?: string;
  createdAt: string;
}

export interface CancelPaymentDto {
  paymentId: string;
  note: string;
}

export interface PaymentEvent {
  paymentEventId: string;
  paymentId: string;
  eventType: string;
  oldStatus?: string | null;
  newStatus?: string | null;
  source: string;
  message?: string | null;
  createdAt: string;
}
