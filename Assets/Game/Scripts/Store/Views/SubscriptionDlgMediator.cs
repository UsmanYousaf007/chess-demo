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
    private string screenContext;
    private string powerUpContext;
    private KeyValuePair<string, object> analyticsFunnelId;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDailougeAnalytic);
        view.restorePurchasesSignal.AddListener(OnRestorePurchases);
        view.purchaseSignal.AddListener(OnPurchase);
        view.showTermsSignal.AddListener(OnTermsClicked);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (!isAvailable)
        {
            view.Init();
        }
        view.SetupPurchaseButton(isAvailable);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG && !view.IsVisible())
        {
            view.Show();

            preferencesModel.timeAtSubscrptionDlgShown = System.DateTime.Now;

            //analytics
            analyticsFunnelId = new KeyValuePair<string, object>("funnel_instance_id", string.Concat(playerModel.id, backendService.serverClock.currentTimestamp));
            analyticsService.ScreenVisit(AnalyticsScreen.subscription_dlg);
            cameFromState = navigatorModel.previousState;
            screenContext = cameFromState.GetType().Equals(typeof(NSLobby)) || cameFromState.GetType().Equals(typeof(NSRateAppDlg)) ? !appInfoModel.isAutoSubscriptionDlgShown ? "lobby_banner" : "auto_popup" : "";
            cameFromScreen = cameFromState.ToString();
            cameFromScreen = cameFromScreen.Remove(0, cameFromScreen.IndexOf("NS") + 2);
            cameFromScreen = screenContext.Equals("auto_popup") ? screenContext : cameFromScreen;
            cameFromScreen = cameFromState.GetType().Equals(typeof(NSMultiplayer)) || cameFromState.GetType().Equals(typeof(NSCPU)) ? powerUpContext : cameFromScreen;
            analyticsService.Event(AnalyticsEventId.subscription_dlg_shown, AnalyticsParameter.context, cameFromScreen);
            hAnalyticsService.LogEvent("subscription_popup_displayed", "subscription", "subscription_popup", cameFromScreen, analyticsFunnelId);
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG)
        {
            analyticsService.Event(AnalyticsEventId.close_subscription_clicked, AnalyticsParameter.context, cameFromScreen);
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
    }

    private void OnRestorePurchases()
    {
        restorePurchasesSignal.Dispatch();

#if UNITY_IOS
        hAnalyticsService.LogEvent("restore_ios_iap_clicked", "subscription", "subscription_popup");
#endif

    }

    private void OnPurchase()
    {
        purchaseStoreItemSignal.Dispatch(view.key, true);
        hAnalyticsService.LogEvent("start_trial_clicked", "subscription", "subscription_popup", analyticsFunnelId);
        analyticsService.Event(AnalyticsEventId.get_free_trial_clicked, AnalyticsParameter.context, cameFromScreen);
    }

    [ListensTo(typeof(ShowProcessingSignal))]
    public void OnShowProcessingUI(bool show, bool showProcessingUi)
    {
        view.ShowProcessing(show, showProcessingUi);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible())
        {
            analyticsService.Event(AnalyticsEventId.subscription_purchased, AnalyticsParameter.context, cameFromScreen);
            hAnalyticsService.LogEvent("close_popup_clicked", "subscription", "subscription_popup", "subscribe_button", analyticsFunnelId);
            OnCloseDailogue();
        }
    }

    [ListensTo(typeof(SelectTierSignal))]
    public void OnTierSelected(string key)
    {
        view.key = key;
    }

    private void OnTermsClicked()
    {
        hAnalyticsService.LogEvent("terms_clicked", "subscription", "subscription_popup", analyticsFunnelId);
        analyticsService.Event(AnalyticsEventId.terms_clicked, AnalyticsParameter.context, cameFromScreen);
    }

    [ListensTo(typeof(SetSubscriptionContext))]
    public void OnSetContext(string gameType, string powerUp)
    {
        powerUpContext = string.Concat(gameType, powerUp);
    }
}
