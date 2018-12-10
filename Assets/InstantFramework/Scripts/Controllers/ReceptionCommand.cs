/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ReceptionCommand : Command
    {
        // Dispatch signals
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public InitBackendOnceSignal initBackendOnceSignal { get; set; }
        [Inject] public GetInitDataSignal getInitDataSignal  { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }


        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            CommandBegin();

            getInitDataSignal.Dispatch();

        }
            
        private void OnGetInitDataComplete()
        {
            // Check version information. Prompt the player if an update is needed.
            if (metaDataModel.appInfo.appBackendVersionValid == false)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE);
                CommandEnd();
                return;
            }

            initBackendOnceSignal.Dispatch();
            loadLobbySignal.Dispatch();

            if (facebookService.isLoggedIn())
            {
                refreshFriendsSignal.Dispatch();
                refreshCommunitySignal.Dispatch();
            }

            SendAnalytics();

            CommandEnd();
        }

        private void SendAnalytics()
        {
            if (facebookService.isLoggedIn())
            {
                analyticsService.FacebookLoggedIn();

                int facebookFriendCount = 0;
                int communityFriendCount = 0;
                foreach (KeyValuePair<string, Friend> kvp in playerModel.friends)
                {
                    Friend friend = kvp.Value;
                    if (friend.friendType == GSBackendKeys.Friend.TYPE_SOCIAL)
                    {
                        facebookFriendCount++;
                    }
                    else if (friend.friendType == GSBackendKeys.Friend.TYPE_COMMUNITY)
                    {
                        communityFriendCount++;
                    }
                }

                analyticsService.FacebookFriendCount(facebookFriendCount);
                analyticsService.CommunityFriendCount(communityFriendCount);
            }

            analyticsService.PlayerRating(playerModel.eloScore);
            analyticsService.ActiveLongMatchCount(matchInfoModel.matches.Count);
        }

        private void CommandBegin()
        {
            Retain();
            getInitDataCompleteSignal.AddListener(OnGetInitDataComplete);
        }

        private void CommandEnd()
        {
            getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
            Release();
        }

    }
}