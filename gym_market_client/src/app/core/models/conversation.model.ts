export interface Conversation {
  conversationId: number;
  conversationName: string;
  hasNewMessage: boolean;
  lastMessage: string;
  avatar: string;
}

export interface Message {
  id?: number;
  content: string;
  senderId: string | null;
  sentAt?: string;
  conversationId: number;
  avatar?: string;
}

export interface SendMessageDto {
  conversationId: number;
  content: string;
  senderId: string;
}

export interface CreateConversationDto {
  trainerId: string;
  studentId: string;
}
