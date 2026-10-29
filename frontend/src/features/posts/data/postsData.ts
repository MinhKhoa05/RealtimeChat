export interface PostComment {
  id: string;
  author: string;
  avatarType: "steve" | "pink_cat" | "boy" | "user_khoa" | "dog" | "diamond" | "creeper" | "chest";
  content: string;
  createdAt: string;
}

export interface PostItem {
  id: string;
  authorId: string;
  authorName: string;
  avatarType: "steve" | "pink_cat" | "boy" | "user_khoa" | "dog" | "creeper" | "diamond";
  content: string;
  createdAt: string;
  isMyPost?: boolean;
  likesCount: number;
  isLiked?: boolean;
  commentsCount: number;
  comments?: PostComment[];
  mediaUrl?: string;
  mediaType?: "image";
}

export const samplePosts: PostItem[] = [
  {
    id: "post-1",
    authorId: "me",
    authorName: "Khoa Nguyễn",
    avatarType: "user_khoa",
    content: "Vừa hoàn thiện xong hệ thống BlockChat UI! Giao diện mới nhìn rất sạch sẽ, tone xanh phẳng hiện đại 🚀",
    createdAt: "15 phút trước",
    isMyPost: true,
    likesCount: 12,
    isLiked: true,
    commentsCount: 3,
    comments: [
      { id: "c-1", author: "Alex Mason", avatarType: "steve", content: "Đẹp quá bạn ơi! 👍", createdAt: "10 phút trước" },
      { id: "c-2", author: "Luna Lovegood", avatarType: "pink_cat", content: "Màu xanh dịu mắt ghê ✨", createdAt: "5 phút trước" },
    ],
  },
  {
    id: "post-2",
    authorId: "u-1",
    authorName: "Alex Mason",
    avatarType: "steve",
    content: "Hôm nay cùng team xây dựng xong lâu đài đá đỏ trong server Minecraft survival. Ai muốn vào tham quan không nè?",
    createdAt: "2 giờ trước",
    isMyPost: false,
    likesCount: 24,
    isLiked: false,
    commentsCount: 5,
    mediaUrl: "preview_ui_2.png",
    mediaType: "image",
    comments: [
      { id: "c-3", author: "Tony Stark", avatarType: "diamond", content: "Kiến trúc ấn tượng đấy!", createdAt: "1 giờ trước" },
    ],
  },
  {
    id: "post-3",
    authorId: "u-2",
    authorName: "Luna Lovegood",
    avatarType: "pink_cat",
    content: "Cuối tuần vui vẻ nhé cả nhà! Đừng quên nghỉ ngơi và thư giãn sau những giờ coding căng thẳng 🌸☕",
    createdAt: "Hôm qua lúc 18:30",
    isMyPost: false,
    likesCount: 38,
    isLiked: true,
    commentsCount: 2,
    comments: [],
  },
  {
    id: "post-4",
    authorId: "u-3",
    authorName: "Tony Stark",
    avatarType: "diamond",
    content: "Vừa tối ưu xong thuật toán xử lý dữ liệu thời gian thực. Độ trễ giảm còn dưới 15ms. Tuyệt đối hài lòng!",
    createdAt: "2 ngày trước",
    isMyPost: false,
    likesCount: 56,
    isLiked: false,
    commentsCount: 8,
    comments: [],
  },
];
