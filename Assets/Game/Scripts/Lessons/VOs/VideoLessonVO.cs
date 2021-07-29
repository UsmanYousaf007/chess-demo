/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class VideoLessonVO
    {
        public string name;
        public Sprite icon;
        public string videoId;
        public bool isLocked;
        public float progress;
        public int indexInTopic;
        public int overallIndex;
        public string section;
        public StoreItem storeItem;
        public IPlayerModel playerModel;
        public long duration;
    }
}