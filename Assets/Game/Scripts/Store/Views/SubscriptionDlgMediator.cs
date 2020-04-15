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

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
    [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

    //Models
    [Inject] public INavigatorModel navigatorModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }
    [Inject] public IAppInfoModel appInfoModel { get; set; }

    private string cameFromScreen;
    private NS cameFromState;
    private string screenContext;
    private string powerUpContext;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDailogue);
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
            analyticsService.ScreenVisit(AnalyticsScreen.subscription_dlg);
            cameFromState = navigatorModel.previousState;
            screenContext = cameFromState.GetType().Equals(typeof(NSLobby)) || cameFromState.GetType().Equals(typeof(NSRateAppDlg)) ? !appInfoModel.isAutoSubscriptionDlgShown ? "banner" : "auto_popup" : "";
            hAnalyticsService.LogEvent("subscription_popup_displayed", "menu", "subscription_popup", screenContext);
            cameFromScreen = cameFromState.ToString();
            cameFromScreen = cameFromScreen.Remove(0, cameFromScreen.IndexOf("NS") + 2);
            cameFromScreen = screenContext.Equals("auto_popup") ? screenContext : cameFromScreen;
            cameFromScreen = cameFromState.GetType().Equals(typeof(NSMultiplayer)) || cameFromState.GetType().Equals(typeof(NSCPU)) ? powerUpContext : cameFromScreen;
            analyticsService.Event(AnalyticsEventId.subscription_dlg_shown, AnalyticsParameter.context, cameFromScreen);
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG)
        {
            hAnalyticsService.LogEvent("close_popup_clicked", "menu", "subscription_popup", screenContext);
            analyticsService.Event(AnalyticsEventId.close_subscription_clicked, AnalyticsParameter.context, cameFromScreen);
            view.Hide();
            subscriptionDlgClosedSignal.Dispatch();
        }
    }

    private void OnCloseDailogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
    }

    private void OnRestorePurchases()
    {
        restorePurchasesSignal.Dispatch();

#if UNITY_IOS
        hAnalyticsService.LogEvent("restore_ios_iap_clicked", "menu", "subscription_popup");
#endif

    }

    private void OnPurchase()
    {
        purchaseStoreItemSignal.Dispatch(view.key, true);
        hAnalyticsService.LogEvent("start_trial_clicked", "menu", "subscription_popup", screenContext);
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
        hAnalyticsService.LogEvent("terms_clicked", "menu", "subscription_popup", screenContext);
        analyticsService.Event(AnalyticsEventId.terms_clicked, AnalyticsParameter.context, cameFromScreen);
    }

    [ListensTo(typeof(SetSubscriptionContext))]
    public void OnSetContext(string gameType, string powerUp)
    {
        powerUpContext = string.Concat(gameType, powerUp);
    }
}
