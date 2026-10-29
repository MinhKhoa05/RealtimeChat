import { useState } from "react";
import {
  FileText,
  User,
  Users,
  Heart,
  MessageCircle,
  Share2,
  Image as ImageIcon,
  Send,
  MoreHorizontal,
  Trash2,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import { samplePosts, type PostItem } from "../data/postsData";

type PostSubTab = "feed" | "my_posts";

export function PostsView() {
  const [activeSubTab, setActiveSubTab] = useState<PostSubTab>("feed");
  const [posts, setPosts] = useState<PostItem[]>(samplePosts);
  const [newPostContent, setNewPostContent] = useState("");
  const [commentInputs, setCommentInputs] = useState<{ [postId: string]: string }>({});

  // Filter posts
  const filteredPosts = activeSubTab === "my_posts"
    ? posts.filter((p) => p.isMyPost)
    : posts;

  // Create new post
  const handleCreatePost = () => {
    if (!newPostContent.trim()) return;
    const newPost: PostItem = {
      id: `post-${Date.now()}`,
      authorId: "me",
      authorName: "Khoa Nguyễn",
      avatarType: "user_khoa",
      content: newPostContent.trim(),
      createdAt: "Vừa xong",
      isMyPost: true,
      likesCount: 0,
      isLiked: false,
      commentsCount: 0,
      comments: [],
    };
    setPosts([newPost, ...posts]);
    setNewPostContent("");
  };

  // Toggle like
  const handleToggleLike = (postId: string) => {
    setPosts((prev) =>
      prev.map((p) => {
        if (p.id === postId) {
          const isLiked = !p.isLiked;
          return {
            ...p,
            isLiked,
            likesCount: isLiked ? p.likesCount + 1 : p.likesCount - 1,
          };
        }
        return p;
      }),
    );
  };

  // Delete post
  const handleDeletePost = (postId: string) => {
    setPosts((prev) => prev.filter((p) => p.id !== postId));
  };

  // Add comment
  const handleAddComment = (postId: string) => {
    const text = commentInputs[postId]?.trim();
    if (!text) return;

    setPosts((prev) =>
      prev.map((p) => {
        if (p.id === postId) {
          const newComment = {
            id: `c-${Date.now()}`,
            author: "Khoa Nguyễn",
            avatarType: "user_khoa" as const,
            content: text,
            createdAt: "Vừa xong",
          };
          return {
            ...p,
            commentsCount: p.commentsCount + 1,
            comments: [...(p.comments || []), newComment],
          };
        }
        return p;
      }),
    );

    setCommentInputs({ ...commentInputs, [postId]: "" });
  };

  return (
    <div className="flex h-full flex-1 overflow-hidden bg-[#ffffff] select-none">
      {/* Sub-Sidebar (Cột 2): Phân loại bài viết */}
      <aside className="flex h-full w-[310px] shrink-0 flex-col border-r border-[#dbe6f5] bg-white">
        {/* Header */}
        <div className="flex items-center gap-2 p-3.5 border-b border-[#f1f5f9]">
          <div className="flex size-9 items-center justify-center rounded-xl bg-[#eef3fb] text-[#0052cc]">
            <FileText className="size-5" />
          </div>
          <h2 className="text-sm font-bold text-slate-900">Bảng tin bài viết</h2>
        </div>

        {/* Categories navigation list */}
        <div className="p-2 space-y-1">
          <button
            type="button"
            onClick={() => setActiveSubTab("feed")}
            className={cn(
              "flex w-full items-center gap-3 rounded-xl px-3.5 py-3 text-xs font-semibold transition-all duration-150 text-left",
              activeSubTab === "feed"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <Users className="size-4.5 text-[#0052cc]" />
            <span>Bài viết bạn bè</span>
          </button>

          <button
            type="button"
            onClick={() => setActiveSubTab("my_posts")}
            className={cn(
              "flex w-full items-center gap-3 rounded-xl px-3.5 py-3 text-xs font-semibold transition-all duration-150 text-left",
              activeSubTab === "my_posts"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <User className="size-4.5 text-slate-500" />
            <span>Bài viết của tôi</span>
          </button>
        </div>
      </aside>

      {/* Main Content Area: Feed stream */}
      <main className="flex h-full flex-1 flex-col overflow-hidden bg-[#f4f7fb]">
        {/* Top Header */}
        <header className="flex h-16 shrink-0 items-center justify-between border-b border-[#dbe6f5] bg-white px-6 shadow-2xs">
          <div className="flex items-center gap-3">
            <h1 className="text-base font-bold text-slate-900 tracking-tight">
              {activeSubTab === "feed" ? "Bảng tin bạn bè" : "Bài viết của tôi"}
            </h1>
          </div>
        </header>

        {/* Feed List */}
        <div className="flex-1 overflow-y-auto p-6 space-y-5 max-w-3xl w-full mx-auto">
          {/* Create Post Box */}
          <div className="rounded-2xl border border-slate-200/90 bg-white p-4 shadow-xs">
            <div className="flex items-start gap-3">
              <PixelBlockIcon type="user_khoa" className="size-10 rounded-xl shadow-2xs shrink-0" />
              <div className="flex-1">
                <textarea
                  value={newPostContent}
                  onChange={(e) => setNewPostContent(e.target.value)}
                  placeholder="Khoa ơi, hôm nay bạn có điều gì muốn chia sẻ?"
                  rows={3}
                  className="w-full resize-none bg-[#f8fafc] rounded-xl p-3 text-sm text-slate-800 placeholder:text-slate-400 outline-none border border-transparent focus:border-[#0052cc] focus:bg-white transition"
                />

                <div className="mt-3 flex items-center justify-between pt-2 border-t border-slate-100">
                  <div className="flex items-center gap-2 text-slate-500">
                    <button
                      type="button"
                      className="flex items-center gap-1.5 rounded-lg px-2.5 py-1.5 text-xs font-medium hover:bg-slate-100 transition"
                    >
                      <ImageIcon className="size-4 text-emerald-600" />
                      <span>Hình ảnh</span>
                    </button>
                  </div>

                  <button
                    type="button"
                    onClick={handleCreatePost}
                    disabled={!newPostContent.trim()}
                    className={cn(
                      "flex items-center gap-1.5 rounded-xl px-4 py-2 text-xs font-semibold text-white transition active:scale-95 shadow-xs",
                      newPostContent.trim()
                        ? "bg-[#0052cc] hover:bg-[#0043a8]"
                        : "bg-slate-200 text-slate-400 cursor-not-allowed",
                    )}
                  >
                    <span>Đăng bài</span>
                    <Send className="size-3.5" />
                  </button>
                </div>
              </div>
            </div>
          </div>

          {/* Posts Stream */}
          {filteredPosts.map((post) => (
            <article
              key={post.id}
              className="rounded-2xl border border-slate-200/90 bg-white p-5 shadow-xs space-y-3.5"
            >
              {/* Post Header */}
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <PixelBlockIcon
                    type={post.avatarType}
                    className="size-10 rounded-xl shadow-2xs"
                  />
                  <div>
                    <h3 className="text-sm font-bold text-slate-900 leading-tight">
                      {post.authorName}
                    </h3>
                    <p className="text-[11px] text-slate-400 leading-tight mt-0.5">
                      {post.createdAt}
                    </p>
                  </div>
                </div>

                {post.isMyPost ? (
                  <button
                    type="button"
                    onClick={() => handleDeletePost(post.id)}
                    className="grid size-8 place-items-center rounded-lg text-slate-400 hover:bg-rose-50 hover:text-rose-600 transition"
                    title="Xóa bài viết"
                  >
                    <Trash2 className="size-4" />
                  </button>
                ) : (
                  <button
                    type="button"
                    className="grid size-8 place-items-center rounded-lg text-slate-400 hover:bg-slate-100 hover:text-slate-700 transition"
                  >
                    <MoreHorizontal className="size-4" />
                  </button>
                )}
              </div>

              {/* Post Content */}
              <p className="text-sm text-slate-800 leading-relaxed whitespace-pre-line">
                {post.content}
              </p>

              {/* Post Image (if any) */}
              {post.mediaUrl && (
                <div className="rounded-xl overflow-hidden border border-slate-200">
                  <img
                    src={post.mediaUrl}
                    alt="Post media"
                    className="w-full max-h-80 object-cover"
                  />
                </div>
              )}

              {/* Like / Comment count stats */}
              <div className="flex items-center justify-between text-xs text-slate-400 pt-1 border-t border-slate-100">
                <span>{post.likesCount} lượt thích</span>
                <span>{post.commentsCount} bình luận</span>
              </div>

              {/* Action Buttons */}
              <div className="grid grid-cols-3 gap-1 pt-1 border-t border-slate-100 text-xs font-semibold text-slate-600">
                <button
                  type="button"
                  onClick={() => handleToggleLike(post.id)}
                  className={cn(
                    "flex items-center justify-center gap-2 py-2 rounded-xl transition hover:bg-slate-50",
                    post.isLiked ? "text-rose-600" : "text-slate-600",
                  )}
                >
                  <Heart className={cn("size-4", post.isLiked && "fill-rose-600 text-rose-600")} />
                  <span>{post.isLiked ? "Đã thích" : "Thích"}</span>
                </button>

                <button
                  type="button"
                  className="flex items-center justify-center gap-2 py-2 rounded-xl hover:bg-slate-50 transition"
                >
                  <MessageCircle className="size-4" />
                  <span>Bình luận</span>
                </button>

                <button
                  type="button"
                  className="flex items-center justify-center gap-2 py-2 rounded-xl hover:bg-slate-50 transition"
                >
                  <Share2 className="size-4" />
                  <span>Chia sẻ</span>
                </button>
              </div>

              {/* Comments Section */}
              <div className="space-y-2.5 pt-2 border-t border-slate-100">
                {post.comments && post.comments.length > 0 && (
                  <div className="space-y-2">
                    {post.comments.map((c) => (
                      <div key={c.id} className="flex items-start gap-2.5">
                        <PixelBlockIcon
                          type={c.avatarType}
                          className="size-7 rounded-lg shadow-2xs mt-0.5"
                        />
                        <div className="flex-1 bg-[#f8fafc] rounded-xl px-3 py-2 border border-slate-100">
                          <div className="flex items-center justify-between">
                            <span className="text-xs font-bold text-slate-900">{c.author}</span>
                            <span className="text-[10px] text-slate-400">{c.createdAt}</span>
                          </div>
                          <p className="text-xs text-slate-700 mt-0.5">{c.content}</p>
                        </div>
                      </div>
                    ))}
                  </div>
                )}

                {/* Add Comment Input */}
                <div className="flex items-center gap-2 pt-1">
                  <PixelBlockIcon
                    type="user_khoa"
                    className="size-7 rounded-lg shadow-2xs shrink-0"
                  />
                  <input
                    type="text"
                    value={commentInputs[post.id] || ""}
                    onChange={(e) =>
                      setCommentInputs({ ...commentInputs, [post.id]: e.target.value })
                    }
                    onKeyDown={(e) => e.key === "Enter" && handleAddComment(post.id)}
                    placeholder="Viết bình luận..."
                    className="flex-1 bg-[#f8fafc] rounded-xl px-3 py-1.5 text-xs text-slate-800 placeholder:text-slate-400 outline-none border border-slate-200 focus:border-[#0052cc] focus:bg-white transition"
                  />
                  <button
                    type="button"
                    onClick={() => handleAddComment(post.id)}
                    className="grid size-7.5 place-items-center rounded-xl bg-[#0052cc] text-white hover:bg-[#0043a8] transition shadow-2xs shrink-0"
                  >
                    <Send className="size-3.5" />
                  </button>
                </div>
              </div>
            </article>
          ))}
        </div>
      </main>
    </div>
  );
}
