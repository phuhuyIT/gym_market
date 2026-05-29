export interface LoginResponse {
  success: boolean;
  token: string;
  refreshToken: string;
  message: string;
  errors: string[];
}

export interface SignupResponse {
  success: boolean;
  userId: string;
  message: string;
  errors: string[];
}

export interface UserTokenPayload {
  nameid: string;
  unique_name: string;
  email: string;
  role: string | string[];
  exp: number;
  homePhone?: string;
  trainerId?: string;
  studentId?: string;
  avatar?: string;
}

export interface HttpErrorBody {
  error?: {
    errors?: string[] | Record<string, string[]> | string;
    message?: string;
  };
  message?: string;
}

export interface UserInfo {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  address: string;
  avatar: string;
  status: string | null;
}

export interface UserInfoResponse {
  userInfo: UserInfo;
}

export interface ApiResponse {
  success: boolean;
  message: string;
  errors?: string[];
}

export interface Enable2FAResponse extends ApiResponse {
  sharedKey: string;
  qrCodeUri: string;
}

export interface LockoutStatusResponse extends ApiResponse {
  isLockedOut: boolean;
  lockoutEnd: string | null;
  accessFailedCount: number;
}

export interface AvatarUploadResponse extends ApiResponse {
  avatarUrl: string;
}
