using HUF.Analytics.API;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class SubscriptionDlgMediator : Mediator
{
    // View injection
    [Inject] public SubscriptionDlgView view { get; set; }

    // Services
    [Inject] public TurboLabz.InstantFramework.IAnalyticsService analyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
    [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
    [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

    public override void OnRegister()
    {
        view.closeDailogueSignal.AddListener(OnCloseDailogue);
        view.restorePurchasesSignal.AddListener(OnRestorePurchases);
        view.purchaseSignal.AddListener(OnPurchase);
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            view.Init();
        }
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG)
        {
            view.Show();
            analyticsService.ScreenVisit(AnalyticsScreen.subscription_dlg);

            HAnalytics.LogEvent(AnalyticsEvent.Create("subscription_popup_displayed").ST1("menu").ST2("subscription_popup"));
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG)
        {
            view.Hide();
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
        HAnalytics.LogEvent(AnalyticsEvent.Create("restore_ios_iap_clicked").ST1("menu").ST2("subscription_popup"));
#endif

    }

    private void OnPurchase()
    {
        purchaseStoreItemSignal.Dispatch(view.key, true);

        HAnalytics.LogEvent(AnalyticsEvent.Create("start_trial_clicked").ST1("menu").ST2("subscription_popup"));
    }

    [ListensTo(typeof(ShowProcessingSignal))]
    public void OnShowProcessingUI(bool show, bool showProcessingUi)
    {
        view.ShowProcessing(show, showProcessingUi);
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnSubscriptionPurchased(StoreItem item)
    {
        if (view.IsVisible() && item.key.Equals(view.key))
        {
            OnCloseDailogue();
        }
    }
}
