/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class InboxBar : MonoBehaviour
    {
        public Button button;

        public Image thumbnailBg;
        public Text headingText;
        public Text subHeadingText;
        public Image thumbnail;

        [HideInInspector]
        public long timeStamp;
        public string msgId; 
    }
}
