export interface Message {
    content: string;
    avatar: string;
    conversationId: number;
    senderId: string | null
}