using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealtimeChat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEntitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_calls_users_CallerId",
                table: "calls");

            migrationBuilder.DropForeignKey(
                name: "FK_calls_users_ReceiverId",
                table: "calls");

            migrationBuilder.DropForeignKey(
                name: "FK_conversation_members_conversations_ConversationId",
                table: "conversation_members");

            migrationBuilder.DropForeignKey(
                name: "FK_conversation_members_messages_LastReadMessageId",
                table: "conversation_members");

            migrationBuilder.DropForeignKey(
                name: "FK_conversation_members_users_MemberId",
                table: "conversation_members");

            migrationBuilder.DropForeignKey(
                name: "FK_conversations_media_AvatarMediaId",
                table: "conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_messages_MessageId",
                table: "Mentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_users_UserId",
                table: "Mentions");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_calls_CallId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_conversations_ConversationId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_media_MediaId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_users_SenderId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_relationships_users_TargetUserId",
                table: "relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_relationships_users_UserId",
                table: "relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_share_trip_sessions_conversations_ConversationId",
                table: "share_trip_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_share_trip_sessions_users_OwnerId",
                table: "share_trip_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_sticker_collections_users_OwnerId",
                table: "sticker_collections");

            migrationBuilder.DropForeignKey(
                name: "FK_stickers_media_MediaId",
                table: "stickers");

            migrationBuilder.DropForeignKey(
                name: "FK_stickers_sticker_collections_CollectionId",
                table: "stickers");

            migrationBuilder.DropForeignKey(
                name: "FK_user_live_locations_users_UserId",
                table: "user_live_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_user_sticker_collections_sticker_collections_CollectionId",
                table: "user_sticker_collections");

            migrationBuilder.DropForeignKey(
                name: "FK_user_sticker_collections_users_UserId",
                table: "user_sticker_collections");

            migrationBuilder.DropForeignKey(
                name: "FK_users_media_AvatarMediaId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stickers",
                table: "stickers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_relationships",
                table: "relationships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conversations",
                table: "conversations");

            migrationBuilder.DropIndex(
                name: "IX_conversations_DirectKey",
                table: "conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_calls",
                table: "calls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_sticker_collections",
                table: "user_sticker_collections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_live_locations",
                table: "user_live_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sticker_collections",
                table: "sticker_collections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_share_trip_sessions",
                table: "share_trip_sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_media",
                table: "media");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conversation_members",
                table: "conversation_members");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "stickers",
                newName: "Stickers");

            migrationBuilder.RenameTable(
                name: "relationships",
                newName: "Relationships");

            migrationBuilder.RenameTable(
                name: "messages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "conversations",
                newName: "Conversations");

            migrationBuilder.RenameTable(
                name: "calls",
                newName: "Calls");

            migrationBuilder.RenameTable(
                name: "user_sticker_collections",
                newName: "UserStickerCollections");

            migrationBuilder.RenameTable(
                name: "user_live_locations",
                newName: "UserLiveLocations");

            migrationBuilder.RenameTable(
                name: "sticker_collections",
                newName: "StickerCollections");

            migrationBuilder.RenameTable(
                name: "share_trip_sessions",
                newName: "ShareTripSessions");

            migrationBuilder.RenameTable(
                name: "media",
                newName: "Medias");

            migrationBuilder.RenameTable(
                name: "conversation_members",
                newName: "ConversationMembers");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_users_AvatarMediaId",
                table: "Users",
                newName: "IX_Users_AvatarMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_stickers_PublicId",
                table: "Stickers",
                newName: "IX_Stickers_PublicId");

            migrationBuilder.RenameIndex(
                name: "IX_stickers_MediaId",
                table: "Stickers",
                newName: "IX_Stickers_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_stickers_CollectionId",
                table: "Stickers",
                newName: "IX_Stickers_CollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_relationships_UserId_TargetUserId_Type",
                table: "Relationships",
                newName: "IX_Relationships_UserId_TargetUserId_Type");

            migrationBuilder.RenameIndex(
                name: "IX_relationships_TargetUserId",
                table: "Relationships",
                newName: "IX_Relationships_TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_StickerId",
                table: "Messages",
                newName: "IX_Messages_StickerId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_SenderId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_MediaId",
                table: "Messages",
                newName: "IX_Messages_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ConversationId_CreatedAt",
                table: "Messages",
                newName: "IX_Messages_ConversationId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_messages_CallId",
                table: "Messages",
                newName: "IX_Messages_CallId");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_AvatarMediaId",
                table: "Conversations",
                newName: "IX_Conversations_AvatarMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_calls_ReceiverId",
                table: "Calls",
                newName: "IX_Calls_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_calls_CallerId",
                table: "Calls",
                newName: "IX_Calls_CallerId");

            migrationBuilder.RenameIndex(
                name: "IX_user_sticker_collections_CollectionId",
                table: "UserStickerCollections",
                newName: "IX_UserStickerCollections_CollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_sticker_collections_SharedKey",
                table: "StickerCollections",
                newName: "IX_StickerCollections_SharedKey");

            migrationBuilder.RenameIndex(
                name: "IX_sticker_collections_OwnerId",
                table: "StickerCollections",
                newName: "IX_StickerCollections_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_share_trip_sessions_OwnerId",
                table: "ShareTripSessions",
                newName: "IX_ShareTripSessions_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_share_trip_sessions_ConversationId",
                table: "ShareTripSessions",
                newName: "IX_ShareTripSessions_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_media_StorageKey",
                table: "Medias",
                newName: "IX_Medias_StorageKey");

            migrationBuilder.RenameIndex(
                name: "IX_media_PublicId",
                table: "Medias",
                newName: "IX_Medias_PublicId");

            migrationBuilder.RenameColumn(
                name: "JoinedAt",
                table: "ConversationMembers",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_conversation_members_MemberId",
                table: "ConversationMembers",
                newName: "IX_ConversationMembers_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_conversation_members_LastReadMessageId",
                table: "ConversationMembers",
                newName: "IX_ConversationMembers_LastReadMessageId");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stickers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Relationships",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefreshTokens",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Messages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Mentions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Mentions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Mentions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Conversations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Calls",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "UserStickerCollections",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserStickerCollections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Visibility",
                table: "StickerCollections",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StickerCollections",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShareTripSessions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Medias",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "ConversationMembers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ConversationMembers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stickers",
                table: "Stickers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Relationships",
                table: "Relationships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Calls",
                table: "Calls",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStickerCollections",
                table: "UserStickerCollections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLiveLocations",
                table: "UserLiveLocations",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StickerCollections",
                table: "StickerCollections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShareTripSessions",
                table: "ShareTripSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medias",
                table: "Medias",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMembers",
                table: "ConversationMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_MessageId",
                table: "Mentions",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_DirectKey",
                table: "Conversations",
                column: "DirectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStickerCollections_UserId_CollectionId",
                table: "UserStickerCollections",
                columns: new[] { "UserId", "CollectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMembers_ConversationId_MemberId",
                table: "ConversationMembers",
                columns: new[] { "ConversationId", "MemberId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Calls_Users_CallerId",
                table: "Calls",
                column: "CallerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Calls_Users_ReceiverId",
                table: "Calls",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMembers_Conversations_ConversationId",
                table: "ConversationMembers",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMembers_Messages_LastReadMessageId",
                table: "ConversationMembers",
                column: "LastReadMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMembers_Users_MemberId",
                table: "ConversationMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Medias_AvatarMediaId",
                table: "Conversations",
                column: "AvatarMediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_Messages_MessageId",
                table: "Mentions",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_Users_UserId",
                table: "Mentions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Calls_CallId",
                table: "Messages",
                column: "CallId",
                principalTable: "Calls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Medias_MediaId",
                table: "Messages",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Stickers_StickerId",
                table: "Messages",
                column: "StickerId",
                principalTable: "Stickers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Users_TargetUserId",
                table: "Relationships",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Users_UserId",
                table: "Relationships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShareTripSessions_Conversations_ConversationId",
                table: "ShareTripSessions",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShareTripSessions_Users_OwnerId",
                table: "ShareTripSessions",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StickerCollections_Users_OwnerId",
                table: "StickerCollections",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stickers_Medias_MediaId",
                table: "Stickers",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stickers_StickerCollections_CollectionId",
                table: "Stickers",
                column: "CollectionId",
                principalTable: "StickerCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLiveLocations_Users_UserId",
                table: "UserLiveLocations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Medias_AvatarMediaId",
                table: "Users",
                column: "AvatarMediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStickerCollections_StickerCollections_CollectionId",
                table: "UserStickerCollections",
                column: "CollectionId",
                principalTable: "StickerCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStickerCollections_Users_UserId",
                table: "UserStickerCollections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calls_Users_CallerId",
                table: "Calls");

            migrationBuilder.DropForeignKey(
                name: "FK_Calls_Users_ReceiverId",
                table: "Calls");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMembers_Conversations_ConversationId",
                table: "ConversationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMembers_Messages_LastReadMessageId",
                table: "ConversationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMembers_Users_MemberId",
                table: "ConversationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Medias_AvatarMediaId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_Messages_MessageId",
                table: "Mentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_Users_UserId",
                table: "Mentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Calls_CallId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Medias_MediaId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Stickers_StickerId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Users_TargetUserId",
                table: "Relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Users_UserId",
                table: "Relationships");

            migrationBuilder.DropForeignKey(
                name: "FK_ShareTripSessions_Conversations_ConversationId",
                table: "ShareTripSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShareTripSessions_Users_OwnerId",
                table: "ShareTripSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StickerCollections_Users_OwnerId",
                table: "StickerCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_Stickers_Medias_MediaId",
                table: "Stickers");

            migrationBuilder.DropForeignKey(
                name: "FK_Stickers_StickerCollections_CollectionId",
                table: "Stickers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLiveLocations_Users_UserId",
                table: "UserLiveLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Medias_AvatarMediaId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStickerCollections_StickerCollections_CollectionId",
                table: "UserStickerCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStickerCollections_Users_UserId",
                table: "UserStickerCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stickers",
                table: "Stickers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Relationships",
                table: "Relationships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions");

            migrationBuilder.DropIndex(
                name: "IX_Mentions_MessageId",
                table: "Mentions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_DirectKey",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Calls",
                table: "Calls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStickerCollections",
                table: "UserStickerCollections");

            migrationBuilder.DropIndex(
                name: "IX_UserStickerCollections_UserId_CollectionId",
                table: "UserStickerCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLiveLocations",
                table: "UserLiveLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StickerCollections",
                table: "StickerCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShareTripSessions",
                table: "ShareTripSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medias",
                table: "Medias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMembers",
                table: "ConversationMembers");

            migrationBuilder.DropIndex(
                name: "IX_ConversationMembers_ConversationId_MemberId",
                table: "ConversationMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Stickers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserStickerCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserStickerCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StickerCollections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ShareTripSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ConversationMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ConversationMembers");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Stickers",
                newName: "stickers");

            migrationBuilder.RenameTable(
                name: "Relationships",
                newName: "relationships");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "messages");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "conversations");

            migrationBuilder.RenameTable(
                name: "Calls",
                newName: "calls");

            migrationBuilder.RenameTable(
                name: "UserStickerCollections",
                newName: "user_sticker_collections");

            migrationBuilder.RenameTable(
                name: "UserLiveLocations",
                newName: "user_live_locations");

            migrationBuilder.RenameTable(
                name: "StickerCollections",
                newName: "sticker_collections");

            migrationBuilder.RenameTable(
                name: "ShareTripSessions",
                newName: "share_trip_sessions");

            migrationBuilder.RenameTable(
                name: "Medias",
                newName: "media");

            migrationBuilder.RenameTable(
                name: "ConversationMembers",
                newName: "conversation_members");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_AvatarMediaId",
                table: "users",
                newName: "IX_users_AvatarMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Stickers_PublicId",
                table: "stickers",
                newName: "IX_stickers_PublicId");

            migrationBuilder.RenameIndex(
                name: "IX_Stickers_MediaId",
                table: "stickers",
                newName: "IX_stickers_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Stickers_CollectionId",
                table: "stickers",
                newName: "IX_stickers_CollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Relationships_UserId_TargetUserId_Type",
                table: "relationships",
                newName: "IX_relationships_UserId_TargetUserId_Type");

            migrationBuilder.RenameIndex(
                name: "IX_Relationships_TargetUserId",
                table: "relationships",
                newName: "IX_relationships_TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_StickerId",
                table: "messages",
                newName: "IX_messages_StickerId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "messages",
                newName: "IX_messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_MediaId",
                table: "messages",
                newName: "IX_messages_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId_CreatedAt",
                table: "messages",
                newName: "IX_messages_ConversationId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_CallId",
                table: "messages",
                newName: "IX_messages_CallId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_AvatarMediaId",
                table: "conversations",
                newName: "IX_conversations_AvatarMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_Calls_ReceiverId",
                table: "calls",
                newName: "IX_calls_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Calls_CallerId",
                table: "calls",
                newName: "IX_calls_CallerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserStickerCollections_CollectionId",
                table: "user_sticker_collections",
                newName: "IX_user_sticker_collections_CollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_StickerCollections_SharedKey",
                table: "sticker_collections",
                newName: "IX_sticker_collections_SharedKey");

            migrationBuilder.RenameIndex(
                name: "IX_StickerCollections_OwnerId",
                table: "sticker_collections",
                newName: "IX_sticker_collections_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ShareTripSessions_OwnerId",
                table: "share_trip_sessions",
                newName: "IX_share_trip_sessions_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ShareTripSessions_ConversationId",
                table: "share_trip_sessions",
                newName: "IX_share_trip_sessions_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Medias_StorageKey",
                table: "media",
                newName: "IX_media_StorageKey");

            migrationBuilder.RenameIndex(
                name: "IX_Medias_PublicId",
                table: "media",
                newName: "IX_media_PublicId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "conversation_members",
                newName: "JoinedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMembers_MemberId",
                table: "conversation_members",
                newName: "IX_conversation_members_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMembers_LastReadMessageId",
                table: "conversation_members",
                newName: "IX_conversation_members_LastReadMessageId");

            migrationBuilder.AlterColumn<string>(
                name: "Visibility",
                table: "sticker_collections",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stickers",
                table: "stickers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_relationships",
                table: "relationships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_messages",
                table: "messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentions",
                table: "Mentions",
                columns: new[] { "MessageId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_conversations",
                table: "conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_calls",
                table: "calls",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_sticker_collections",
                table: "user_sticker_collections",
                columns: new[] { "UserId", "CollectionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_live_locations",
                table: "user_live_locations",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sticker_collections",
                table: "sticker_collections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_share_trip_sessions",
                table: "share_trip_sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_media",
                table: "media",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_conversation_members",
                table: "conversation_members",
                columns: new[] { "ConversationId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_conversations_DirectKey",
                table: "conversations",
                column: "DirectKey");

            migrationBuilder.AddForeignKey(
                name: "FK_calls_users_CallerId",
                table: "calls",
                column: "CallerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_calls_users_ReceiverId",
                table: "calls",
                column: "ReceiverId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_conversation_members_conversations_ConversationId",
                table: "conversation_members",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_conversation_members_messages_LastReadMessageId",
                table: "conversation_members",
                column: "LastReadMessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_conversation_members_users_MemberId",
                table: "conversation_members",
                column: "MemberId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_conversations_media_AvatarMediaId",
                table: "conversations",
                column: "AvatarMediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_messages_MessageId",
                table: "Mentions",
                column: "MessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_users_UserId",
                table: "Mentions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_calls_CallId",
                table: "messages",
                column: "CallId",
                principalTable: "calls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_conversations_ConversationId",
                table: "messages",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_media_MediaId",
                table: "messages",
                column: "MediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_stickers_StickerId",
                table: "messages",
                column: "StickerId",
                principalTable: "stickers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_users_SenderId",
                table: "messages",
                column: "SenderId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_relationships_users_TargetUserId",
                table: "relationships",
                column: "TargetUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_relationships_users_UserId",
                table: "relationships",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_share_trip_sessions_conversations_ConversationId",
                table: "share_trip_sessions",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_share_trip_sessions_users_OwnerId",
                table: "share_trip_sessions",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sticker_collections_users_OwnerId",
                table: "sticker_collections",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stickers_media_MediaId",
                table: "stickers",
                column: "MediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stickers_sticker_collections_CollectionId",
                table: "stickers",
                column: "CollectionId",
                principalTable: "sticker_collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_live_locations_users_UserId",
                table: "user_live_locations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_sticker_collections_sticker_collections_CollectionId",
                table: "user_sticker_collections",
                column: "CollectionId",
                principalTable: "sticker_collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_sticker_collections_users_UserId",
                table: "user_sticker_collections",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_media_AvatarMediaId",
                table: "users",
                column: "AvatarMediaId",
                principalTable: "media",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
