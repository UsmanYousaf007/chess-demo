using UnityEngine;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public struct ChatVO
    {
        public ChatMessages chatMessages;
        public string playerId;
        public Sprite playerProfilePic;
        public Sprite opponentProfilePic;
        public string opponentName;
    }
}