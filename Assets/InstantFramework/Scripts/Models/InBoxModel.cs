/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;
using System.Linq;

namespace TurboLabz.InstantFramework
{
    public class InboxModel : IInboxModel
    {
        public Dictionary<string, InboxMessage> items { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            items = new Dictionary<string, InboxMessage>();
        }
    }

    public class InboxMessage
    {
        public string id;
        public string type;
        public string heading;
        public string subHeading;
        public long timeStamp;

        public InboxMessage()
        {
            id = null;
            type = null;
            heading = null;
            subHeading = null;
            timeStamp = 0;
        }
    }
}
