/// @license Propriety <http://license.url>
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
using GameAnalyticsSDK;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        [Inject] public PauseNotificationsSignal pauseNotificationsSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }
        [Inject] public VirtualGoodBoughtSignal virtualGoodBoughtSignal { get; set; }

        private FindViewVO vo;

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
                toggleLeaderboardViewNavButtons.Dispatch(true);

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
            this.vo = vo;
        }


        [ListensTo(typeof(MatchFoundSignal))]
        public void OnMatchFound(ProfileVO vo)
        {
            view.MatchFound(vo);
        }

        public void OnFindMatchTimeout()
        {
            matchAnalyticsSignal.Dispatch(GetFindMatchAnalyticsVO(AnalyticsContext.failed));
            loadLobbySignal.Dispatch();

            //if (vo.isTournamentMatch && vo.isTicketSpent)
            //{
            //    RefundTicket();
            //}

            if (vo.bettingCoins > 0)
            {
                RefundCoins(vo.bettingCoins);
            }
        }

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdatePic(string playerId, Sprite sprite)
        {
            view.SetProfilePicById(playerId, sprite);
        }

        private MatchAnalyticsVO GetFindMatchAnalyticsVO(AnalyticsContext context)
        {
            var matchAnalyticsVO = new MatchAnalyticsVO();
            matchAnalyticsVO.eventID = AnalyticsEventId.match_find;
            matchAnalyticsVO.context = context;

            if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                matchAnalyticsVO.matchType = "classic";
            }
            else if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge.ToString() ||
                     FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random.ToString())
            {
                matchAnalyticsVO.matchType = "5m";
            }
            else if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge10.ToString() ||
                     FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random10.ToString())
            {
                matchAnalyticsVO.matchType = "10m";
            }
            else if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge30.ToString() ||
                     FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random30.ToString())
            {
                matchAnalyticsVO.matchType = "30m";
            }
            else if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge1.ToString() ||
                     FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random1.ToString())
            {
                matchAnalyticsVO.matchType = "1m";
            }
            else if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Challenge3.ToString() ||
                     FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random3.ToString())
            {
                matchAnalyticsVO.matchType = "3m";
            }


            if (FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random10.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random30.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random1.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.Random3.ToString() ||
                FindMatchAction.actionData.action == FindMatchAction.ActionCode.RandomLong.ToString())
            {
                matchAnalyticsVO.friendType = "random";
            }
            else if (playerModel.friends.ContainsKey(FindMatchAction.actionData.opponentId))
            {
                var friendType = playerModel.friends[FindMatchAction.actionData.opponentId].friendType;
                if (friendType.Equals(GSBackendKeys.Friend.TYPE_SOCIAL))
                {
                    matchAnalyticsVO.friendType = "friends_facebook";
                }
                else if (friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE))
                {
                    matchAnalyticsVO.friendType = "friends_community";
                }
                else
                {
                    matchAnalyticsVO.friendType = "community";
                }
            }
            else
            {
                matchAnalyticsVO.friendType = "community";
            }

            return matchAnalyticsVO;
        }

        private void RefundTicket()
        {
            var transactionVO = new VirtualGoodsTransactionVO();
            transactionVO.buyItemShortCode = GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET;
            transactionVO.buyQuantity = 1;
            //virtualGoodBoughtSignal.AddOnce(OnTicketRefunded);
            virtualGoodsTransactionSignal.Dispatch(transactionVO);
        }

        private void RefundCoins(long value)
        {
            var transactionVO = new VirtualGoodsTransactionVO();
            transactionVO.buyItemShortCode = GSBackendKeys.PlayerDetails.COINS;
            transactionVO.buyQuantity = (int)value;
            virtualGoodsTransactionSignal.Dispatch(transactionVO);
            virtualGoodBoughtSignal.AddOnce((key, qty) =>
                analyticsService.ResourceEvent(GAResourceFlowType.Source, key, qty, "refund", "match_not_found")
            );
        }

        private void OnTicketRefunded(string key)
        {
            analyticsService.ResourceEvent(GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(key).ToString(), 1, "refund", "match_not_found");
            preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_FREE][key] += 1;
        }
    }
}
