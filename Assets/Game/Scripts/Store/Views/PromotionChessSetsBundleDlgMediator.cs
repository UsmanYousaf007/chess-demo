using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class PromotionChessSetsBundleDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionChessSetsBundleDlgView view { get; set; }

    // Services
    [Inject] public IPromotionsService promotionsService { get; set; }
    [Inject] public IAnalyticsService analyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    //Models
    [Inject] public IAppInfoModel appInfoModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }

    private IPromise<AdsResult> promise;
    private string screenContext;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDialogue);
        view.purchaseSignal.AddListener(OnPurchase);
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

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_CHESS_SETS_BUNDLE_DLG)
        {
            view.Show();
            analyticsService.Event(AnalyticsEventId.promotion_dlg_shown, AnalyticsContext.themes_pack);
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_CHESS_SETS_BUNDLE_DLG)
        {
            view.Hide();
        }
    }

    private void OnCloseDialogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        promotionsService.LoadPromotion();
    }

    private void OnPurchase()
    {
        purchaseStoreItemSignal.Dispatch(view.key, true);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible() && item.key.Equals(view.key))
        {
            OnCloseDialogue();
            analyticsService.Event(AnalyticsEventId.promotion_dlg_purchased, AnalyticsContext.themes_pack);
        }
    }
}
