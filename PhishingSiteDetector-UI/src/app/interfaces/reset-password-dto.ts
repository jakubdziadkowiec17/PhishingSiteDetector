export interface ResetPasswordDTO {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}