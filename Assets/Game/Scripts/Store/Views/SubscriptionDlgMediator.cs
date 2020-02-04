﻿using strange.extensions.mediation.impl;
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

    //Models
    [Inject] public INavigatorModel navigatorModel { get; set; }

    private string cameFromScreen;

    public override void OnRegister()
    {
        view.InitOnce();
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
        if (viewId == NavigatorViewId.SUBSCRIPTION_DLG && !view.IsVisible())
        {
            view.Show();

            analyticsService.ScreenVisit(AnalyticsScreen.subscription_dlg);
            hAnalyticsService.LogEvent("subscription_popup_displayed", "menu", "subscription_popup");
            cameFromScreen = navigatorModel.previousState.ToString();
            cameFromScreen = cameFromScreen.Remove(0, cameFromScreen.IndexOf("NS") + 2);
            analyticsService.Event(AnalyticsEventId.subscription_dlg_shown, AnalyticsParameter.context, cameFromScreen);
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
        hAnalyticsService.LogEvent("restore_ios_iap_clicked", "menu", "subscription_popup");
#endif

    }

    private void OnPurchase()
    {
        purchaseStoreItemSignal.Dispatch(view.key, true);
        hAnalyticsService.LogEvent("start_trial_clicked", "menu", "subscription_popup");
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
        if (view.IsVisible() && item.key.Equals(view.key))
        {
            analyticsService.Event(AnalyticsEventId.subscription_purchased, AnalyticsParameter.context, cameFromScreen);
            OnCloseDailogue();
        }
    }
}
