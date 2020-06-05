using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

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
        [Inject] public ManageBlockedFriendsSignal manageBlockedFriendsSignal { get; set; }
        [Inject] public UnblockFriendSignal unblockFriendSignal { get; set; }

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

        [ListensTo(typeof(UpdateManageBlockedFriendsViewSignal))]
        public void OnUpdateView(Dictionary<string, Friend> blockedFriends)
        {
            view.UpdateView(blockedFriends);
        }

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdateFriendPic(string playerId, Sprite sprite)
        {
            view.UpdateFriendPic(playerId, sprite);
        }

        private void OnBackSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnSubmitSearchSignal(string filter)
        {
            manageBlockedFriendsSignal.Dispatch(filter, false);
        }

        private void OnUnblockFriendSignal(string friendId)
        {
            unblockFriendSignal.Dispatch(friendId);
        }
    }
}
