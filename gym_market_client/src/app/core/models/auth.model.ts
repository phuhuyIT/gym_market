export interface LoginResponse {
  success: boolean;
  token: string;
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
  role: string;
  exp: number;
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
