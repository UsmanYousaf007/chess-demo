using System;
using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

[System.CLSCompliant(false)]
public class PromotionRemoveAdsDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionRemoveAdsDlgView view { get; set; }

    // Services
    [Inject] public IPromotionsService promotionsService { get; set; }
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public ISchedulerService schedulerService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    //Models
    [Inject] public IAppInfoModel appInfoModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDialogue);
        view.purchaseSignal.AddListener(OnPurchase);
        view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        view.OnStoreAvailable(isAvailable);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_DLG || viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG)
        {
            view.isOnSale = viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG;
            view.Show();
            analyticsService.Event(AnalyticsEventId.promotion_dlg_shown, view.isOnSale? AnalyticsContext.remove_ads_fire_sale : AnalyticsContext.remove_ads);
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_DLG || viewId == NavigatorViewId.PROMOTION_REMOVE_ADS_SALE_DLG)
        {
            view.Hide();
        }
    }

    private void OnCloseDialogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        promotionsService.LoadPromotion();
    }

    private void OnPurchase(string shortCode)
    {
        purchaseStoreItemSignal.Dispatch(shortCode, true);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible() && (item.key.Equals(view.shortCode) || item.key.Equals(view.saleShortCode)))
        {
            OnCloseDialogue();
            analyticsService.Event(AnalyticsEventId.promotion_dlg_purchased, view.isOnSale ? AnalyticsContext.remove_ads_fire_sale : AnalyticsContext.remove_ads);
        }
    }

    private void OnSchedulerSubscriptionToggle(Action callback, bool subscribe)
    {
        if (subscribe)
        {
            schedulerService.Subscribe(callback);
        }
        else
        {
            schedulerService.UnSubscribe(callback);
        }
    }
}
