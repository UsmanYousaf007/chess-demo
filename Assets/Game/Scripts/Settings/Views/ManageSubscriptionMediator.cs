using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class ManageSubscriptionMediator : Mediator
{
    // View injection
    [Inject] public ManageSubscriptionView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }
    [Inject] public IStoreService storeService { get; set; }
    [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

    // Dispatch signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

    //Models
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    public override void OnRegister()
    {
        view.Init();
        view.backSignal.AddListener(OnBack);
        view.switchPlanSignal.AddListener(OnSwitchPlan);
    }


    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.MANAGE_SUBSCRIPTION)
        {
            view.Show();
            analyticsService.ScreenVisit(AnalyticsScreen.manage_subscription);
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.MANAGE_SUBSCRIPTION)
        {
            view.Hide();
        }
    }

    private void OnBack()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
    }

    private void OnSwitchPlan()
    {
        var monthlySubscription = metaDataModel.store.items[GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG];
        var annualSubscription = metaDataModel.store.items[GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SHOP_TAG];
        var isMonthlyActive = playerModel.subscriptionType.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG);

        if (isMonthlyActive)
        {
            hAnalyticsService.LogEvent("clicked", "settings", "", "upgrade_subscription_yearly");
            storeService.UpgardeSubscription(monthlySubscription.remoteProductId, annualSubscription.remoteProductId);
        }
        else
        {
            hAnalyticsService.LogEvent("clicked", "settings", "", "upgrade_subscription_monthly");
            storeService.UpgardeSubscription(annualSubscription.remoteProductId, monthlySubscription.remoteProductId);
        }
    }

    [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
    public void OnPlanSwitched(StoreItem item)
    {
        if (view.IsVisible())
        {
            view.Setup();
        }
    }
}
