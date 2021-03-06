using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

public class SubscriptionDlgMediator : Mediator
{
    // View injection
    [Inject] public SubscriptionDlgView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
    [Inject] public IBackendService backendService { get; set; }
    [Inject] public IAutoSubscriptionDailogueService autoSubscriptionDailogueService { get; set; }
    [Inject] public IPromotionsService promotionsService { get; set; }
    [Inject] public ISchedulerService schedulerService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
    [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

    //Models
    [Inject] public INavigatorModel navigatorModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }
    [Inject] public IAppInfoModel appInfoModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    private string cameFromScreen;
    private NS cameFromState;
    private string context;
    private KeyValuePair<string, object> analyticsFunnelId;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDailougeAnalytic);
        view.restorePurchasesSignal.AddListener(OnRestorePurchases);
        view.purchaseSignal.AddListener(OnPurchase);
        view.showTermsSignal.AddListener(OnTermsClicked);
        view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        view.OnStoreAvailable(isAvailable);
        view.SetupPurchaseButton(isAvailable);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if ((viewId == NavigatorViewId.SUBSCRIPTION_DLG || viewId == NavigatorViewId.SUBSCRIPTION_SALE_DLG) && !view.IsVisible())
        {
            view.isOnSale = viewId == NavigatorViewId.SUBSCRIPTION_SALE_DLG;
            view.Show();

            preferencesModel.timeAtSubscrptionDlgShown = System.DateTime.Now;

            //analytics
            analyticsFunnelId = new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, backendService.serverClock.currentTimestamp));
            analyticsService.ScreenVisit(AnalyticsScreen.subscription_dlg);
            cameFromState = navigatorModel.previousState;
            cameFromScreen = cameFromState.ToString();
            cameFromScreen = CollectionsUtil.GetContextFromState(cameFromScreen.Remove(0, cameFromScreen.IndexOf("NS") + 2));
            cameFromScreen = appInfoModel.isAutoSubscriptionDlgShown ? autoSubscriptionDailogueService.IsShownFirstTime() ? "install_popup" : "auto_popup" : cameFromScreen;
            var cameFromCustomContext = cameFromState.GetType().Equals(typeof(NSMultiplayer)) || cameFromState.GetType().Equals(typeof(NSCPU)) ||
                cameFromState.GetType().Equals(typeof(NSLessonTopics)) || cameFromState.GetType().Equals(typeof(NSLessonsView)) || cameFromState.GetType().Equals(typeof(NSLessonVideo));
            cameFromScreen = cameFromCustomContext ? context : cameFromScreen;
            cameFromScreen = view.isOnSale ? AnalyticsContext.annual_mega_sale.ToString() : cameFromScreen; 
            analyticsService.Event(AnalyticsEventId.subscription_dlg_shown, AnalyticsParameter.context, cameFromScreen);
            hAnalyticsService.LogEvent("subscription_popup_displayed", "subscription", "subscription_popup", cameFromScreen, analyticsFunnelId);

            if (appInfoModel.isAutoSubscriptionDlgShown)
            {
                analyticsService.Event(AnalyticsEventId.promotion_dlg_shown, view.isOnSale ? AnalyticsContext.annual_mega_sale : AnalyticsContext.subscription);
            }
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG || viewId == NavigatorViewId.SUBSCRIPTION_SALE_DLG)
        {
            view.Hide();
            subscriptionDlgClosedSignal.Dispatch();
        }
    }

    private void OnCloseDailougeAnalytic()
    {
        hAnalyticsService.LogEvent("close_popup_clicked", "subscription", "subscription_popup", "x_button", analyticsFunnelId);
        OnCloseDailogue();
    }

    private void OnCloseDailogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        promotionsService.LoadPromotion();
    }

    private void OnRestorePurchases()
    {
        restorePurchasesSignal.Dispatch();

#if UNITY_IOS
        hAnalyticsService.LogEvent("restore_ios_iap_clicked", "subscription", "subscription_popup");
#endif

    }

    private void OnPurchase(string shortCode)
    {
        purchaseStoreItemSignal.Dispatch(shortCode, true);
        hAnalyticsService.LogEvent("start_trial_clicked", "subscription", "subscription_popup", analyticsFunnelId);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible() && item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG))
        {
            analyticsService.Event(AnalyticsEventId.subscription_dlg_purchased, AnalyticsParameter.context, cameFromScreen);
            hAnalyticsService.LogEvent("close_popup_clicked", "subscription", "subscription_popup", "subscribe_button", analyticsFunnelId);

            if (appInfoModel.isAutoSubscriptionDlgShown)
            {
                var context = item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG) ? AnalyticsContext.monthly_sub :
                    item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SHOP_TAG) ? AnalyticsContext.annual_sub : AnalyticsContext.annual_mega_sale;
                analyticsService.Event(AnalyticsEventId.promotion_dlg_purchased, context);
            }

            OnCloseDailogue();
        }
    }

    [ListensTo(typeof(SelectTierSignal))]
    public void OnTierSelected(string key)
    {
        view.shortCode = key;
        view.offerBg.childAlignment = UnityEngine.TextAnchor.MiddleRight;
        view.offerBg.childAlignment = UnityEngine.TextAnchor.MiddleCenter;
    }

    private void OnTermsClicked()
    {
        hAnalyticsService.LogEvent("terms_clicked", "subscription", "subscription_popup", analyticsFunnelId);
    }

    [ListensTo(typeof(SetSubscriptionContext))]
    public void OnSetContext(string context)
    {
        this.context = context;
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
