/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TurboLabz.InstantFramework
{
    public class InboxModel : IInboxModel
    {
        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        public Dictionary<string, InboxMessage> items { get; set; }
        public DateTime lastFetchedTime { get; set; }

        public int inboxMessageCount { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        public InboxMessage GetTournamentRewardMessage(string tournamentId)
        {
            foreach (InboxMessage msg in items.Values)
            {
                if (msg.tournamentId == tournamentId)
                {
                    return msg;
                }
            }

            return null;
        }

        private void Reset()
        {
            lastFetchedTime = DateTime.MinValue;
            items = new Dictionary<string, InboxMessage>();
        }
    }

    public class InboxMessage
    {
        public string id;
        public string type;
        public bool isDaily;
        public string heading;
        public string subHeading;
        public long timeStamp;
        public string tournamentType;
        public string tournamentId;
        public string chestType;
        public string league;
        public Dictionary<string, int> rewards;
        public int trophiesCount;
        public int rankCount;
        public long startTime;

        public InboxMessage()
        {
            id = null;
            type = null;
            heading = null;
            subHeading = null;
            timeStamp = 0;
            tournamentType = null;
            tournamentId = null;
            chestType = null;
            league = null;
            rewards = new Dictionary<string, int>();
            trophiesCount = 0;
            rankCount = 0;
            startTime = 0;
        }
    }
}
