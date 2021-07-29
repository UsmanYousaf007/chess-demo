
using System;
using System.Collections.Generic;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public class LoadRewardDlgCommand : Command
    {
        // Parameters
        [Inject] public string inboxMessageId { get; set; }
        [Inject] public Signal onCloseSignal { get; set; }

        // Models
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getJoinedTournamentLeaderboardSignal { get; set; }
        [Inject] public UpdateRewardDlgViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public UpdateChampionshipResultDlgSignal updateChampionshipResultDlgViewSignal { get; set; }
        [Inject] public UpdateLeaguePromotionDlgViewSignal updateLeaguePromotionDlgViewSignal { get; set; }

        public override void Execute()
        {
            if (inboxModel.items.ContainsKey(inboxMessageId) == false)
            {
                return;
            }

            InboxMessage msg = inboxModel.items[inboxMessageId];
            RewardDlgVO vo = null;

            if (msg.type == "RewardTournamentEnd")
            {
                vo = BuildVORewardRewardTournamentEnd(inboxMessageId);

                vo.msgId = inboxMessageId;
                vo.tournamentId = msg.tournamentId;
                vo.onCloseSignal = onCloseSignal;
                //getJoinedTournamentLeaderboardSignal.Dispatch(msg.tournamentId, false);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHAMPIONSHIP_RESULT_DLG);
                updateChampionshipResultDlgViewSignal.Dispatch(vo);

                return;
            }
            else if (msg.type == "RewardLeaguePromotion")
            {
                vo = BuildVORewardRewardLeaguePromotion(inboxMessageId);

                vo.msgId = inboxMessageId;
                vo.onCloseSignal = onCloseSignal;
                updateLeaguePromotionDlgViewSignal.Dispatch(vo);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LEAGUE_PROMOTION_DLG);

                return;
            }
            else if (msg.type == "RewardDailyLeague")
            {
                vo = BuildVORewardRewardDailyLeague(inboxMessageId);

                vo.msgId = inboxMessageId;
                vo.onCloseSignal = onCloseSignal;
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_DAILY_REWARD_DLG);
                updateRewardDlgViewSignal.Dispatch(vo);

                return;
            }
            else if (msg.type == "RewardDailySubscription")
            {
                vo = BuildVORewardDailySubscription(inboxMessageId);
            }

            vo.msgId = inboxMessageId;
            vo.onCloseSignal = onCloseSignal;
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG);
            updateRewardDlgViewSignal.Dispatch(vo);
        }

        private RewardDlgVO BuildVORewardDailySubscription(string msgId)
        {
            RewardDlgVO vo = new RewardDlgVO("RewardDailySubscription");
            InboxMessage msg = inboxModel.items[msgId];
            
            AddRewardsToVO(vo, msg.rewards);

            return vo;
        }
        private RewardDlgVO BuildVORewardRewardDailyLeague(string msgId)
        {
            RewardDlgVO vo = new RewardDlgVO("RewardDailyLeague");
            InboxMessage msg = inboxModel.items[msgId];
            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());

            vo.league = msg.league;
            vo.leagueGradient = leagueAssets.textUnderlaySprite;

            AddRewardsToVO(vo, msg.rewards);

            return vo;
        }
        private RewardDlgVO BuildVORewardRewardLeaguePromotion(string msgId)
        {
            RewardDlgVO vo = new RewardDlgVO("RewardLeaguePromotion");
            InboxMessage msg = inboxModel.items[msgId];
            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());

            vo.league = msg.league;
            vo.leagueGradient = leagueAssets.textUnderlaySprite;
            vo.playerProfile = new ProfileVO();
            vo.playerProfile.playerPic = picsModel.GetPlayerPic(playerModel.id);
            vo.playerProfile.avatarColorId = playerModel.avatarBgColorId;
            vo.playerProfile.leagueBorder = leagueAssets.ringSprite;
            vo.playerProfile.avatarId = playerModel.avatarId;

            AddRewardsToVO(vo, msg.rewards);

            return vo;
        }
        private RewardDlgVO BuildVORewardRewardTournamentEnd(string msgId)
        {
            RewardDlgVO vo = new RewardDlgVO("RewardTournamentEnd");
            InboxMessage msg = inboxModel.items[msgId];

            if (msg.tournamentType == TournamentConstants.TournamentType.MIN_1) vo.tournamentName = TournamentConstants.TournamentName.MIN_1;
            else if (msg.tournamentType == TournamentConstants.TournamentType.MIN_5) vo.tournamentName = TournamentConstants.TournamentName.MIN_5;
            else if (msg.tournamentType == TournamentConstants.TournamentType.MIN_10) vo.tournamentName = TournamentConstants.TournamentName.MIN_10;

            if (msg.chestType == TournamentConstants.ChestType.COMMON) vo.chestName = "Common Chest";
            if (msg.chestType == TournamentConstants.ChestType.EPIC) vo.chestName = "Epic Chest";
            if (msg.chestType == TournamentConstants.ChestType.RARE) vo.chestName = "Rare Chest";

            vo.trophiesCount = msg.trophiesCount;
            vo.rankCount = msg.rankCount;

            ChestIconsContainer container = ChestIconsContainer.Load();
            vo.chestImage = container.GetChest(msg.chestType, true);

            AddRewardsToVO(vo, msg.rewards);

            return vo;
        }

        private Sprite GetRewardImage(string shortCode)
        {
            Sprite sprite = SpriteBank.container.GetSprite(shortCode);
            if (sprite == null)
            {
                sprite = SpriteBank.container.GetSprite("Smile");
            }
            return sprite;
        }

        private void AddRewardsToVO(RewardDlgVO vo, Dictionary<string, int> rewards)
        {
            foreach (KeyValuePair<string, int> item in rewards)
            {
                vo.AddRewardItem(item.Key, item.Value, GetRewardImage(item.Key));
            }
        }
    }
}
