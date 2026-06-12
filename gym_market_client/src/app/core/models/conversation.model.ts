export interface Conversation {
  conversationId: number;
  conversationName: string;
  hasNewMessage: boolean;
  lastMessage: string;
  lastMessageAt?: string | null;
  avatar: string;
  isGroup: boolean;
  role?: string;
  memberCount?: number;
  otherUserId?: string | null;
  isOnline?: boolean;
  lastSeen?: string | null;
}

export interface Message {
  id?: number;
  content: string;
  senderId: string | null;
  senderName?: string;
  sentAt?: string;
  conversationId: number;
  avatar?: string;
  type?: string;
}

export interface SendMessageDto {
  conversationId: number;
  content: string;
  senderId: string;
}

export interface CreateConversationDto {
  senderId: string;
  recieveId: string;
}

export interface CreateGroupDto {
  name: string;
  avatarUrl?: string;
  memberIds: string[];
}

export interface UpdateGroupDto {
  conversationId: number;
  name?: string;
  avatarUrl?: string;
}

export interface AddMembersDto {
  conversationId: number;
  userIds: string[];
}

export interface UpdateMemberRoleDto {
  conversationId: number;
  userId: string;
  role: string;
}

export interface GroupMember {
  userId: string;
  fullName: string;
  avatar: string;
  role: string;
}

export interface UserSearchResult {
  id: string;
  fullName: string;
  avatar: string;
  email: string;
}
