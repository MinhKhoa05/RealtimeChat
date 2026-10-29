export type NavTab = "messages" | "friends" | "calls" | "posts" | "stickers" | "settings";
export type ChatFilter = "all" | "groups" | "friends";

export interface ChatItem {
  id: string;
  name: string;
  avatarType: "grass_block" | "steve" | "pink_cat" | "dog" | "diamond" | "chest" | "boy" | "creeper" | "user_khoa";
  lastMessage: string;
  time: string;
  unreadCount?: number;
  type: "group" | "friend";
  online?: boolean;
}

export interface Reaction {
  emoji: string;
  count: number;
}

export interface Attachment {
  type: "file" | "image";
  name: string;
  size?: string;
}

export interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  avatarType: "steve" | "pink_cat" | "boy" | "user_khoa" | "dog";
  content?: string;
  time: string;
  isMine?: boolean;
  isRead?: boolean;
  attachment?: Attachment;
  dateDivider?: string;
}

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

export interface RecentPost {
  id: string;
  author: string;
  avatarType: string;
  timeAgo: string;
  text: string;
  hasImage?: boolean;
}

export const sampleChats: ChatItem[] = [
  {
    id: "team-project",
    name: "Team Project",
    avatarType: "grass_block",
    lastMessage: "Minh: mai họp lúc 9h nhé!",
    time: "14:02",
    unreadCount: 3,
    type: "group",
  },
  {
    id: "khoa-nguyen",
    name: "Khoa Nguyễn",
    avatarType: "user_khoa",
    lastMessage: "ok ok, để t gửi sau",
    time: "13:48",
    unreadCount: 1,
    type: "friend",
    online: true,
  },
  {
    id: "aniimo-fans",
    name: "Aniimo Fans",
    avatarType: "pink_cat",
    lastMessage: "Đạt: con này dễ thương v...",
    time: "12:20",
    type: "group",
  },
  {
    id: "family",
    name: "Family",
    avatarType: "dog",
    lastMessage: "Mẹ: ăn uống đầy đủ nhé",
    time: "09:30",
    unreadCount: 2,
    type: "group",
  },
  {
    id: "game-zone",
    name: "Game Zone",
    avatarType: "diamond",
    lastMessage: "Hoàng: ai chơi server k?",
    time: "Hôm qua",
    type: "group",
  },
  {
    id: "random-talk",
    name: "Random Talk",
    avatarType: "chest",
    lastMessage: "Phát: 👏",
    time: "Hôm qua",
    type: "group",
  },
];

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

export const samplePosts: RecentPost[] = [
  {
    id: "p1",
    author: "Khoa",
    avatarType: "user_khoa",
    timeAgo: "2 ngày trước",
    text: "Cùng nhau hoàn thiện project nhé!",
    hasImage: true,
  },
];

export const sampleChatMessages: ChatMessage[] = [
  {
    id: "msg-1",
    senderId: "minh",
    senderName: "Minh",
    avatarType: "steve",
    content: "mai họp lúc 9h nhé!",
    time: "14:02",
    dateDivider: "Hôm nay",
  },
  {
    id: "msg-2",
    senderId: "minh",
    senderName: "Minh",
    avatarType: "steve",
    time: "14:03",
    attachment: {
      type: "file",
      name: "design-prototype.fig",
      size: "12.4 MB",
    },
  },
  {
    id: "msg-3",
    senderId: "linh",
    senderName: "Linh",
    avatarType: "pink_cat",
    content: "okie 👌",
    time: "14:05",
  },
  {
    id: "msg-4",
    senderId: "khoa",
    senderName: "Khoa",
    avatarType: "user_khoa",
    content: "mình thêm phần chat như này được không nhỉ?",
    time: "14:06",
    isMine: true,
    isRead: true,
  },
  {
    id: "msg-5",
    senderId: "minh",
    senderName: "Minh",
    avatarType: "dog",
    content: "được đó, nhìn xinh ghê",
    time: "14:07",
  },
  {
    id: "msg-6",
    senderId: "dat",
    senderName: "Đạt",
    avatarType: "pink_cat",
    content: "style Minecraft hợp nè :))",
    time: "14:08",
  },
  {
    id: "msg-7",
    senderId: "khoa",
    senderName: "Khoa",
    avatarType: "user_khoa",
    content: "tối t làm thử giao diện nhé",
    time: "14:10",
    isMine: true,
    isRead: true,
  },
];
