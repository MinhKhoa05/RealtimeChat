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
