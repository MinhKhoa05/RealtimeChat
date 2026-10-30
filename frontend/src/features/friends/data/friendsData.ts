export interface FriendItem {
  id: string;
  name: string;
  avatarType: "steve" | "pink_cat" | "boy" | "user_khoa" | "dog" | "creeper" | "diamond" | "chest";
  online?: boolean;
  statusText?: string;
  group?: string;
}

export interface GroupItem {
  id: string;
  name: string;
  memberCount: number;
  avatarType: "grass_block" | "diamond" | "chest" | "creeper" | "steve" | "dog" | "pink_cat";
  lastActive: string;
  role?: string;
}

export interface BlockedItem {
  id: string;
  name: string;
  avatarType: "steve" | "boy" | "creeper" | "dog";
  blockedDate: string;
}

export type DiscoveryReasonType = "mutual_and_groups" | "mutual_friends" | "shared_groups" | "new_member" | "general_suggestion";

export interface SuggestedUser {
  id: string;
  name: string;
  username: string;
  avatarType: "steve" | "pink_cat" | "boy" | "user_khoa" | "dog" | "creeper" | "diamond" | "chest";
  mutualFriendsCount?: number;
  sharedGroupsCount?: number;
  isNewMember?: boolean;
  reasonType: DiscoveryReasonType;
  bio?: string;
}

// Mock fictional character friends
export const sampleFriendsList: FriendItem[] = [
  { id: "f-1", name: "Alex Mason", avatarType: "steve", online: true, statusText: "Khám phá Redstone", group: "A" },
  { id: "f-2", name: "Arthur Dent", avatarType: "dog", online: false, statusText: "Offline 1 giờ trước", group: "A" },
  { id: "f-3", name: "Boba Fett", avatarType: "boy", online: true, statusText: "Sẵn sàng phiêu lưu", group: "B" },
  { id: "f-4", name: "Bruce Wayne", avatarType: "user_khoa", online: false, statusText: "Đang tuần tra đêm", group: "B" },
  { id: "f-5", name: "Clark Kent", avatarType: "boy", online: true, statusText: "Viết bài Daily Planet", group: "C" },
  { id: "f-6", name: "Cyber Creeper", avatarType: "creeper", online: false, statusText: "Sss... nạp năng lượng", group: "C" },
  { id: "f-7", name: "Diana Prince", avatarType: "diamond", online: true, statusText: "Luyện tập kỹ năng", group: "D" },
  { id: "f-8", name: "Doc Brown", avatarType: "chest", online: true, statusText: "Đang chỉnh cỗ máy", group: "D" },
  { id: "f-9", name: "Frodo Baggins", avatarType: "dog", online: false, statusText: "Đang ở Shire", group: "F" },
  { id: "f-10", name: "Harry Potter", avatarType: "boy", online: true, statusText: "Thực hành phép thuật", group: "H" },
  { id: "f-11", name: "Luna Lovegood", avatarType: "pink_cat", online: true, statusText: "Ngắm sao đêm ✨", group: "L" },
  { id: "f-12", name: "Neo Anderson", avatarType: "steve", online: false, statusText: "Offline", group: "N" },
  { id: "f-13", name: "Peter Parker", avatarType: "boy", online: true, statusText: "Chụp ảnh tòa soạn", group: "P" },
  { id: "f-14", name: "Tony Stark", avatarType: "diamond", online: true, statusText: "Nâng cấp bộ giáp", group: "T" },
];

// Mock fictional generic groups
export const sampleFriendGroups: GroupItem[] = [
  { id: "g-1", name: "Minecraft Builders Guild ⛏️", memberCount: 1420, avatarType: "grass_block", lastActive: "5 phút trước" },
  { id: "g-2", name: "Pixel Art & Game Design Club", memberCount: 860, avatarType: "chest", lastActive: "15 phút trước" },
  { id: "g-3", name: "Open Source Tech Developers", memberCount: 340, avatarType: "diamond", lastActive: "40 phút trước" },
  { id: "g-4", name: "BlockCraft Survival Hardcore", memberCount: 75, avatarType: "creeper", lastActive: "1 giờ trước" },
  { id: "g-5", name: "Web3 & Cloud Architecture", memberCount: 210, avatarType: "diamond", lastActive: "2 giờ trước" },
  { id: "g-6", name: "Indie Game Creators Hub", memberCount: 450, avatarType: "chest", lastActive: "Hôm qua" },
  { id: "g-7", name: "Retro 8-Bit Music Lab", memberCount: 180, avatarType: "pink_cat", lastActive: "Hôm qua" },
  { id: "g-8", name: "Algorithms & Data Structures Study", memberCount: 620, avatarType: "steve", lastActive: "2 ngày trước" },
  { id: "g-9", name: "Design System & Figma Wizards", memberCount: 390, avatarType: "grass_block", lastActive: "3 ngày trước" },
  { id: "g-10", name: "CyberSecurity WhiteHats", memberCount: 510, avatarType: "creeper", lastActive: "Tuần trước" },
];

// Mock fictional friend requests
export const sampleFriendRequests = [
  { id: "req-1", name: "Samwise Gamgee", avatarType: "boy" as const, mutualFriends: 4, time: "Hôm qua" },
  { id: "req-2", name: "Hermione Granger", avatarType: "pink_cat" as const, mutualFriends: 8, time: "2 ngày trước" },
];

// Mock fictional blocked list
export const sampleBlockedList: BlockedItem[] = [
  { id: "b-1", name: "Dark Skeleton", avatarType: "creeper", blockedDate: "12/10/2026" },
  { id: "b-2", name: "Zombie Invader", avatarType: "boy", blockedDate: "05/09/2026" },
];

// Mock suggested users for Friend Discovery (Khám phá bạn bè)
export const sampleSuggestedUsers: SuggestedUser[] = [
  {
    id: "sug-1",
    name: "Lê Minh Tuấn",
    username: "tuan.le_craft",
    avatarType: "steve",
    mutualFriendsCount: 5,
    sharedGroupsCount: 3,
    reasonType: "mutual_and_groups",
    bio: "Đam mê xây dựng Redstone và Pixel Art 🧱",
  },
  {
    id: "sug-2",
    name: "Phạm Hải Đăng",
    username: "dang.haipham",
    avatarType: "boy",
    mutualFriendsCount: 2,
    reasonType: "mutual_friends",
    bio: "Frontend Developer & Gamer",
  },
  {
    id: "sug-3",
    name: "Nguyễn Thảo Vy",
    username: "vy.nguyen_art",
    avatarType: "pink_cat",
    sharedGroupsCount: 3,
    reasonType: "shared_groups",
    bio: "Họa sĩ minh họa 2D & Pixel enthusiast ✨",
  },
  {
    id: "sug-4",
    name: "Đỗ Gia Huy",
    username: "huy.do_newbie",
    avatarType: "dog",
    isNewMember: true,
    reasonType: "new_member",
    bio: "Vừa tham gia BlockChat, rất vui được làm quen!",
  },
  {
    id: "sug-5",
    name: "Trần Bảo An",
    username: "baoan.tran",
    avatarType: "diamond",
    reasonType: "general_suggestion",
    bio: "Thích nghe nhạc lofi & đọc sách công nghệ",
  },
  {
    id: "sug-6",
    name: "Hoàng Minh Trí",
    username: "tri.hoang_pixel",
    avatarType: "chest",
    mutualFriendsCount: 4,
    sharedGroupsCount: 1,
    reasonType: "mutual_and_groups",
    bio: "Game designer độc lập",
  },
  {
    id: "sug-7",
    name: "Vũ Phương Linh",
    username: "linh.vu_design",
    avatarType: "pink_cat",
    mutualFriendsCount: 3,
    reasonType: "mutual_friends",
    bio: "UI/UX Designer, yêu thích sự tối giản",
  },
  {
    id: "sug-8",
    name: "Bùi Quốc Khánh",
    username: "khanh.bui_code",
    avatarType: "creeper",
    sharedGroupsCount: 4,
    reasonType: "shared_groups",
    bio: "Backend Golang & C# lover ⚡",
  },
  {
    id: "sug-9",
    name: "Đặng Mai Chi",
    username: "maichi.dang",
    avatarType: "pink_cat",
    isNewMember: true,
    reasonType: "new_member",
    bio: "Newbie tại BlockChat! Chào mọi người 👋",
  },
  {
    id: "sug-10",
    name: "Ngô Quang Huy",
    username: "huy.ngo_explorer",
    avatarType: "steve",
    reasonType: "general_suggestion",
    bio: "Khám phá những vùng đất và công nghệ mới",
  },
  {
    id: "sug-11",
    name: "Trịnh Thiên Hương",
    username: "huong.trinh_music",
    avatarType: "diamond",
    mutualFriendsCount: 1,
    sharedGroupsCount: 2,
    reasonType: "mutual_and_groups",
    bio: "Nhà sản xuất âm nhạc 8-bit & Synthwave",
  },
  {
    id: "sug-12",
    name: "Lâm Nhật Nam",
    username: "nam.lam_gamer",
    avatarType: "boy",
    mutualFriendsCount: 6,
    reasonType: "mutual_friends",
    bio: "Chơi game cùng bạn bè mỗi tối cuối tuần 🎮",
  },
];
