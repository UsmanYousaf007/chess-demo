using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public struct PromotionVO
    {
        public int cycleIndex;
        public string key;
        public delegate bool Condition();
        public Condition condition;
        public delegate void OnClick(string key = null);
        public OnClick onClick;
    }
}