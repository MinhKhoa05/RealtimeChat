export type ChatFilter = "all" | "groups" | "friends";

export interface ConversationItem {
  id: string;
  name: string;
  avatarType: "grass_block" | "steve" | "pink_cat" | "dog" | "diamond" | "chest" | "boy" | "creeper" | "user_khoa";
  lastMessage: string;
  time: string;
  unreadCount?: number;
  type: "group" | "friend";
  online?: boolean;
}

export const sampleConversations: ConversationItem[] = [
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
