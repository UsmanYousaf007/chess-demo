using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class ManageSubscriptionMediator : Mediator
{
    // View injection
    [Inject] public ManageSubscriptionView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }

    // Dispatch signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

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

    }
}
