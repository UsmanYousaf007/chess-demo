using System.Collections.Generic;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    [System.Serializable]
    public class VideoActiveInventoryItem
    {
        public string shopItemKey;
        public string kind;
        public float progress;

        public VideoActiveInventoryItem(string shopItemKey, string kind, float progress)
        {
            this.shopItemKey = shopItemKey;
            this.kind = kind;
            this.progress = progress;
        }
    }
}
