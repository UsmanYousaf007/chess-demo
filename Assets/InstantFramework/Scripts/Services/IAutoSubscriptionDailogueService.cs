namespace TurboLabz.InstantFramework
{
    public interface IAutoSubscriptionDailogueService
    {
        bool CanShow();
        void Show();
        bool IsShownFirstTime();
    }
}
