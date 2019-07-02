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
        public string avatarId;
        public string avatarBgColorId;
    }
}