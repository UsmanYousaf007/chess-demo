namespace TurboLabz.InstantFramework
{
    [System.CLSCompliantAttribute(false)]
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

    [System.CLSCompliantAttribute(false)]
    public struct PromoionDlgVO
    {
        public string key;
        public NavigatorEvent navigatorEvent;
        public delegate bool Condition();
        public Condition condition;
        public bool isOnSale;
    }
}