export interface UserProfile {
  id: string;
  displayName: string;
  username: string;
  bio: string;
  phone: string;
  gender: "Nam" | "Nữ" | "Khác";
  birthday: string;
  avatarType: "steve" | "pink_cat" | "boy" | "user_khoa" | "dog";
  coverImage?: string;
  location: string;
  joinedDate: string;
}

export const initialUserProfile: UserProfile = {
  id: "user-khoa",
  displayName: "Khoa Nguyễn",
  username: "khoa.nguyen_2026",
  bio: "Lập trình viên Fullstack & Đam mê Minecraft Pixel Art 🎮",
  phone: "0912 345 678",
  gender: "Nam",
  birthday: "15/08/2002",
  avatarType: "user_khoa",
  location: "Hồ Chí Minh, Việt Nam",
  joinedDate: "Tháng 3, 2024",
};
