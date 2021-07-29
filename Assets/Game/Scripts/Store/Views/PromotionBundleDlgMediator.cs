using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

[System.CLSCompliant(false)]
public class PromotionBundleDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionBundleDlgView view { get; set; }

    // Dispatch Signals
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

    // Services
    [Inject] public IPromotionsService promotionsService { get; set; }
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public IBackendService backendService { get; set; }

    //Models
    [Inject] public IPlayerModel playerModel { get; set; }

    private AnalyticsContext analyticsContext;
    private long timeAtDlgShown;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDialogue);
        view.purchaseSignal.AddListener(OnPurchase);
        analyticsContext = CollectionsUtil.GetContextFromString(view.key);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_BUNDLE_DLG)
        {
            if (view.key == playerModel.dynamicBundleToDisplay)
            {
                view.Show();
                analyticsService.Event(AnalyticsEventId.promotion_dlg_shown, analyticsContext);
                analyticsService.Event("ux_salepopup_shown", analyticsContext);
                timeAtDlgShown = backendService.serverClock.currentTimestamp;
            }
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_BUNDLE_DLG)
        {
            view.Hide();
        }
    }

    private void OnPurchase()
    {
        var timePreBuyNow = (backendService.serverClock.currentTimestamp - timeAtDlgShown) / 1000.0f;
        analyticsService.Event("ux_salepopup_tapbuynow", analyticsContext);
        analyticsService.ValueEvent("ux_salepopup_timeprebuynow", analyticsContext, timePreBuyNow);
        purchaseStoreItemSignal.Dispatch(view.key, true);
    }

    private void OnCloseDialogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        promotionsService.LoadPromotion();
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            view.Init();
        }
        view.SetupPurchaseButton(isAvailable);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible() && view.key.Equals(item.key))
        {
            //Analytics
            var context = item.displayName.Replace(' ', '_').ToLower();
            analyticsService.Event(AnalyticsEventId.promotion_dlg_purchased, analyticsContext);
            analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "promotion", $"launch_pop_up_{context}_gems");
            analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)item.currency4Cost, "promotion", $"launch_pop_up_{context}_coins");

            OnCloseDialogue();
        }
    }
}
