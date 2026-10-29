export interface Member {
  id: string;
  name: string;
  avatarType: string;
}

export interface SharedFile {
  id: string;
  name: string;
  size: string;
  date?: string;
}

export const teamMembers: Member[] = [
  { id: "m1", name: "Minh", avatarType: "dog" },
  { id: "m2", name: "Linh", avatarType: "user_khoa" },
  { id: "m3", name: "Đạt", avatarType: "pink_cat" },
  { id: "m4", name: "Khoa", avatarType: "boy" },
  { id: "m5", name: "Hoàng", avatarType: "steve" },
];

export const sampleFiles: SharedFile[] = [
  { id: "f1", name: "spec.docx", size: "1.2 MB" },
];
