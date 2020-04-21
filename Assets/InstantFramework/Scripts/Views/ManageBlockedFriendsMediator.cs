using System.Collections.Generic;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ManageBlockedFriendsMediator : Mediator
    {
        // View injection
        [Inject] public ManageBlockedFriendsView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.backButtonPressedSignal.AddListener(OnBackSignal);
            view.onSubmitSearchSignal.AddListener(OnSubmitSearchSignal);
            view.onUnblockButtonPressedSignal.AddListener(OnUnblockFriendSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MANAGE_BLOCKED_FRIENDS)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.manage_blocked_friends);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MANAGE_BLOCKED_FRIENDS)
            {
                view.Hide();
            }
        }

        public void OnUpdateView(List<Friend> blockedFriends)
        {
            view.UpdateView(blockedFriends);
        }

        private void OnBackSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnSubmitSearchSignal(string filter)
        {

        }

        private void OnUnblockFriendSignal(string friendId)
        {

        }
    }
}
