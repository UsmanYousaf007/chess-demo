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

    private IPromise<AdsResult> promise;

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
    public void OnShowView(IPromise<AdsResult> promise)
    {
        this.promise = promise;
        view.Show();
    }

    private void OnCloseDailogue()
    {
        view.Hide();
        promise.Dispatch(AdsResult.FINISHED);
    }

    private void OnPurchase()
    {
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
