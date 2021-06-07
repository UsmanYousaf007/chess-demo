using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

[System.CLSCompliant(false)]
public class PromotionBundleDlgMediator : PromotionBundleMediator
{
    // View injection
    [Inject] public PromotionBundleDlgView view { get; set; }

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDialogue);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_BUNDLE_DLG)
        {
            if (view.key == playerModel.dynamicBundleToDisplay)
            {
                view.Show();
                analyticsService.Event(AnalyticsEventId.promotion_dlg_shown, AnalyticsContext.elite);
            }
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_BUNDLE_DLG)
        {
            if (view.key == playerModel.dynamicBundleToDisplay)
            {
                view.Hide();
            }
        }
    }

    private void OnCloseDialogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        promotionsService.LoadPromotion();
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible() && view.key.Equals(item.key))
        {
            OnCloseDialogue();

            //Analytics
            var context = item.displayName.Replace(' ', '_').ToLower();
            analyticsService.Event(AnalyticsEventId.promotion_dlg_purchased, AnalyticsContext.elite);
            analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "promotion", $"{context}_gems");
            analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)item.currency4Cost, "promotion", $"{context}_coins");
        }
    }
}
