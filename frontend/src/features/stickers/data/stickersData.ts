export interface StickerItem {
  id: string;
  name: string;
  emoji: string;
  imageUrl?: string;
}

export interface StickerCollection {
  id: string;
  name: string;
  description: string;
  coverEmoji: string;
  isMyCollection?: boolean;
  isSaved?: boolean;
  sharedKey: string;
  stickersCount: number;
  stickers: StickerItem[];
}

export const sampleCollections: StickerCollection[] = [
  {
    id: "col-1",
    name: "Pixel Minecraft Mobs",
    description: "Bộ nhãn dán các sinh vật huyền thoại Minecraft phong cách Pixel Art",
    coverEmoji: "🟩",
    isMyCollection: true,
    isSaved: true,
    sharedKey: "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    stickersCount: 6,
    stickers: [
      { id: "s-1", name: "Creeper Sss", emoji: "🟩" },
      { id: "s-2", name: "Diamond Block", emoji: "💎" },
      { id: "s-3", name: "Chest", emoji: "📦" },
      { id: "s-4", name: "Steve Happy", emoji: "🧑" },
      { id: "s-5", name: "Dog Bone", emoji: "🐶" },
      { id: "s-6", name: "Sword Attack", emoji: "⚔️" },
    ],
  },
  {
    id: "col-2",
    name: "Cute Anime Chibi",
    description: "Biểu cảm siêu đáng yêu của mèo hồng và các nhân vật chibi",
    coverEmoji: "🐱",
    isMyCollection: false,
    isSaved: true,
    sharedKey: "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
    stickersCount: 6,
    stickers: [
      { id: "s-7", name: "Meo Meo", emoji: "🐱" },
      { id: "s-8", name: "Tym tym", emoji: "💖" },
      { id: "s-9", name: "Ngủ Zzz", emoji: "😴" },
      { id: "s-10", name: "Khóc nhè", emoji: "😭" },
      { id: "s-11", name: "Yayyy", emoji: "🎉" },
      { id: "s-12", name: "Ngại ngùng", emoji: "🥺" },
    ],
  },
  {
    id: "col-3",
    name: "Dev Coding Memes",
    description: "Những khoảnh khắc dở khóc dở cười của dân lập trình",
    coverEmoji: "💻",
    isMyCollection: true,
    isSaved: true,
    sharedKey: "c9bf9e57-1685-4c89-bafb-ff5af830be8a",
    stickersCount: 5,
    stickers: [
      { id: "s-13", name: "Bug No Fix", emoji: "🐛" },
      { id: "s-14", name: "Code All Night", emoji: "☕" },
      { id: "s-15", name: "Deploy Prod", emoji: "🚀" },
      { id: "s-16", name: "Git Push Force", emoji: "💥" },
      { id: "s-17", name: "LGTM", emoji: "✅" },
    ],
  },
  {
    id: "col-4",
    name: "Pixel Daily Reactions",
    description: "Bộ phản hồi nhanh dạng 8-bit sống động cho hội thoại",
    coverEmoji: "⭐",
    isMyCollection: false,
    isSaved: false,
    sharedKey: "3f2a1b9c-8e4d-4567-9abc-def012345678",
    stickersCount: 5,
    stickers: [
      { id: "s-18", name: "Like", emoji: "👍" },
      { id: "s-19", name: "Love", emoji: "❤️" },
      { id: "s-20", name: "Fire", emoji: "🔥" },
      { id: "s-21", name: "100 Points", emoji: "💯" },
      { id: "s-22", name: "Clap", emoji: "👏" },
    ],
  },
];
