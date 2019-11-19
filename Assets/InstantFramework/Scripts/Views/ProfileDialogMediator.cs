/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class ProfileDialogMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BlockFriendSignal blockFriendSignal { get; set; }
        [Inject] public NewFriendSignal newFriendSignal { get; set; }
        [Inject] public RemoveCommunityFriendSignal removeCommunityFriendSignal { get; set; }
        [Inject] public CancelSearchResultSignal cancelSearchResultSignal { get; set; }
        [Inject] public LoadChatSignal loadChatSignal { get; set; }

        // View injection
        [Inject] public ProfileDialogView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.closeBtn.onClick.AddListener(OnClose);
            view.blockUserSignal.AddListener(OnBlock);
            view.addFriendSignal.AddListener(OnAddFriend);
            view.friendRemoveSignal.AddListener(OnRemoveFriend);
            view.chatSignal.AddListener(OnChat);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PROFILE_DLG) 
            {
                view.Show();
                analyticsService.Event(AnalyticsEventId.tap_long_match_profile);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PROFILE_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateProfileDialogSignal))]
        public void OnUpdateProfileDialog(ProfileDialogVO vo)
        {
            view.UpdateProfileDialog(vo);
        }

        private void OnClose()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnBlock(string playerId)
        {
            OnClose();
            blockFriendSignal.Dispatch(playerId);
        }

        private void OnChat(string playerId)
        {
            loadChatSignal.Dispatch(playerId, false);
        }

        private void OnAddFriend(string playerId)
        {
            if(!playerModel.friends.ContainsKey(playerId))
            {
                newFriendSignal.Dispatch(playerId, true);
                cancelSearchResultSignal.Dispatch();
            }
            else
            {
                Debug.Log("Already your friend ::: " + playerId);
                OnRemoveFriend(playerId);
                OnAddFriend(playerId);
            }
        }

        private void OnRemoveFriend(string playerId)
        {
            if (playerModel.friends.ContainsKey(playerId))
            {
                removeCommunityFriendSignal.Dispatch(playerId);
            }
            else
            {
                Debug.Log("not your friend ::: " + playerId);
            }
        }
    }
}
