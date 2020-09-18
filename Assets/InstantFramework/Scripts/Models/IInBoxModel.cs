/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using UnityEngine;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public interface IInboxModel
    {
        Dictionary<string, InboxMessage> items { get; set; }
        DateTime lastFetchedTime { get; set; }
        int inboxMessageCount { get; set; }
        InboxMessage GetTournamentRewardMessage(string tournamentId);
    }
}
