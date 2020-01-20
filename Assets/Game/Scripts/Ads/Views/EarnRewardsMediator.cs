using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

public class EarnRewardsMediator : Mediator
{
    // View injection
    [Inject] public EarnRewardsView view { get; set; }

    // Services
    [Inject] public IAnalyticsService analyticsService { get; set; }

    // Dispatch Signals
    [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

    public override void OnRegister()
    {
        view.closeDialogueSignal.AddListener(OnCloseDailogue);
    }

    [ListensTo(typeof(NavigatorShowViewSignal))]
    public void OnShowView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.EARN_REWARDS_DLG)
        {
            view.Show();
        }
    }

    [ListensTo(typeof(NavigatorHideViewSignal))]
    public void OnHideView(NavigatorViewId viewId)
    {
        if (viewId == NavigatorViewId.EARN_REWARDS_DLG)
        {
            view.Hide();
        }
    }

    [ListensTo(typeof(StoreAvailableSignal))]
    public void OnStoreAvailable(bool isAvailable)
    {
        if (isAvailable)
        {
            view.SetupRewardBar();
        }
    }

    private void OnCloseDailogue()
    {
        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
    }
}
