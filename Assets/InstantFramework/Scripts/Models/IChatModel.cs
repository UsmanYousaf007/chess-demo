﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IChatModel
    {
        void Reset();
        bool AddChat(string opponentId, ChatMessage message, bool isBackupMessage);
        ChatMessages GetChat(string opponentId);
        void ClearChat(string opponentId);
        Dictionary<string, bool> hasUnreadMessages { get; set; }
        string lastSavedChatId { get; set; }
        bool preloadingMessagesCompleted { get; set; }
    }
}
