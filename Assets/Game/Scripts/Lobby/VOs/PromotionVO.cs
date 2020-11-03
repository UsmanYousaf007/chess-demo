namespace TurboLabz.InstantFramework
{
    public struct PromotionVO
    {
        public int cycleIndex;
        public string key;
        public delegate bool Condition();
        public Condition condition;
        public delegate void OnClick();
        public OnClick onClick;
        public AnalyticsContext analyticsContext;
    }
}