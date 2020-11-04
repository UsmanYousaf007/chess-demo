namespace TurboLabz.InstantFramework
{
    public struct PromotionVO
    {
        public int cycleIndex;
        public string key;
        public delegate bool Condition();
        public Condition condition;
        public delegate void OnClick();
        public UnityEngine.Events.UnityAction onClick;
        public AnalyticsContext analyticsContext;
    }
}