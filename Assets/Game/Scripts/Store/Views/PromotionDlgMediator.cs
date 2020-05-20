using strange.extensions.mediation.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class PromotionDlgMediator : Mediator
{
    // View injection
    [Inject] public PromotionDlgView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    //Models
    [Inject] public IAppInfoModel appInfoModel { get; set; }
    [Inject] public IPreferencesModel preferencesModel { get; set; }

    private IPromise<AdsResult> promise;
    private string screenContext;

    public override void OnRegister()
    {
        view.InitOnce();
        view.closeDailogueSignal.AddListener(OnCloseDailogue);
        view.purchaseSignal.AddListener(OnPurchase);
        view.restorePurchasesSignal.AddListener(OnRestorePurchases);
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

    [ListensTo(typeof(ShowPromotionDlgSignal))]
    public void OnShowView(IPromise<AdsResult> promise, InternalAdType internalAdType)
    {
        this.promise = promise;
        appInfoModel.internalAdType = internalAdType;
        screenContext = internalAdType == InternalAdType.FORCED_ON_WIN ? "victory_popup" : "internal";
        preferencesModel.timeAtSubscrptionDlgShown = System.DateTime.Now;
        view.Show();
        hAnalyticsService.LogEvent("internal_ad_displayed", "monetization", "internal_fullscreen", screenContext);
        analyticsService.Event(AnalyticsEventId.subscription_dlg_shown, AnalyticsParameter.context, screenContext);
    }

    [ListensTo(typeof(ClosePromotionDlgSignal))]
    public void OnCloseDaiglogueSignal()
    {
        OnCloseDailogue();
    }

    private void OnCloseDailogue()
    {
        view.Hide();
        appInfoModel.internalAdType = InternalAdType.NONE;
        hAnalyticsService.LogEvent("internal_ad_closed", "monetization", "internal_fullscreen", screenContext);
        analyticsService.Event(AnalyticsEventId.close_subscription_clicked, AnalyticsParameter.context, screenContext);

        if (promise != null)
        {
            promise.Dispatch(AdsResult.FINISHED);
        }
    }

    private void OnPurchase()
    {
        hAnalyticsService.LogEvent("internal_ad_clicked", "monetization", "internal_fullscreen", screenContext);
        analyticsService.Event(AnalyticsEventId.get_free_trial_clicked, AnalyticsParameter.context, screenContext);
        purchaseStoreItemSignal.Dispatch(view.key, true);
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
            analyticsService.Event(AnalyticsEventId.subscription_purchased, AnalyticsParameter.context, screenContext);
            OnCloseDailogue();
        }
    }

    private void OnRestorePurchases()
    {
        restorePurchasesSignal.Dispatch();

#if UNITY_IOS
        hAnalyticsService.LogEvent("restore_ios_iap_clicked", "menu", "subscription_popup");
#endif

    }

    [ListensTo(typeof(SelectTierSignal))]
    public void OnTierSelected(string key)
    {
        view.key = key;
    }
}
