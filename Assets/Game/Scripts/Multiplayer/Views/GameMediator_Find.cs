﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]


using TurboLabz.InstantFramework;
using UnityEngine;
using TurboLabz.TLUtils;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        [Inject] public PauseNotificationsSignal pauseNotificationsSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }

        public void OnRegisterFind()
        {
            view.InitFind();
            view.findMatchTimeoutSignal.AddListener(OnFindMatchTimeout);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowFind(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_FIND_DLG) 
            {
                view.ShowFind();
                pauseNotificationsSignal.Dispatch(true);
                view.FindMatchTimeoutEnable(true, 30);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideFind(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_FIND_DLG)
            {
                view.HideFind();
                pauseNotificationsSignal.Dispatch(false);
            }
        }

        [ListensTo(typeof(UpdateFindViewSignal))]
        public void UpdateFind(FindViewVO vo)
        {
            view.FindMatchTimeoutEnable(true, vo.timeoutSeconds);
            view.UpdateFind(vo);
        }


        [ListensTo(typeof(MatchFoundSignal))]
        public void OnMatchFound(ProfileVO vo)
        {
            view.MatchFound(vo);
        }

        public void OnFindMatchTimeout()
        {
            MatchAnalyticsVO matchAnalyticsVO = new MatchAnalyticsVO();
            matchAnalyticsVO.context = AnalyticsContext.failed;

            if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random.ToString() || FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random10.ToString()
                || FindMatchAction.actionData.action == FindMatchAction.ActionCode.RandomLong.ToString())
                matchAnalyticsVO.friendType = "random";
            else
            {
                var friend = playerModel.GetFriend(FindMatchAction.actionData.opponentId);
                matchAnalyticsVO.friendType = friend.friendType;
            }

            matchAnalyticsVO.eventID = AnalyticsEventId.match_find;

            if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                matchAnalyticsVO.matchType = "classic";
            }
            else
            {
                if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge.ToString() || FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random.ToString())
                    matchAnalyticsVO.matchType = "5m";
                else if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge10.ToString() || FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random10.ToString())
                    matchAnalyticsVO.matchType = "10m";
            }

            matchAnalyticsSignal.Dispatch(matchAnalyticsVO);

            /*if (FindMatchAction.isMatchRequestedWithFriend)
            {
                analyticsService.Event(AnalyticsEventId.quickmatch_direct_request_timeout_ingame);

            }else if(FindMatchAction.isRandomLongMatch)
            {
                analyticsService.Event(AnalyticsEventId.match_timer_runs_out, AnalyticsContext.random_long_match);
            }*/
            loadLobbySignal.Dispatch();
        }

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdatePic(string playerId, Sprite sprite)
        {
            view.SetProfilePicById(playerId, sprite);
        }
    }
}
