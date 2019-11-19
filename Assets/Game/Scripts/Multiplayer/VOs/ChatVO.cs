using UnityEngine;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public struct ChatVO
    {
        public ChatMessages chatMessages;
        public string playerId;
        public string opponentId;
        public Sprite playerProfilePic;
        public Sprite opponentProfilePic;
        public string opponentName;
        public bool hasUnreadMessages;
        public int unreadMessagesCount;
        public string avatarId;
        public string avatarBgColorId;
        public string oppAvatarId;
        public string oppAvatarBgColorId;
        public bool isOnline;
        public bool isActive;
        public bool inGame;
        public bool isChatEnabled;
    }
}