
using System;
using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public class LoadRewardDlgCommand : Command
    {
        // Parameters
        [Inject] public string inboxMessageId { get; set; }

        // Models
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateRewardDlgViewSignal updateRewardDlgViewSignal { get; set; }

        public override void Execute()
        {
            InboxMessage msg = inboxModel.items[inboxMessageId];
            RewardDlgVO vo = null;

            if (msg.type == "RewardTournamentEnd")
            {
                vo = BuildVORewardRewardTournamentEnd(inboxMessageId);
            }
            else if (msg.type == "RewardLeaguePromotion")
            {
                vo = BuildVORewardRewardLeaguePromotion(inboxMessageId);
            }
            else if (msg.type == "RewardDailyLeague")
            {
                vo = BuildVORewardRewardDailyLeague(inboxMessageId);
            }
            else if (msg.type == "RewardDailySubscription")
            {
                vo = BuildVORewardDailySubscription(inboxMessageId);
            }

            vo.msgId = inboxMessageId;
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

            vo.league = msg.league;
            AddRewardsToVO(vo, msg.rewards);

            return vo;
        }
        private RewardDlgVO BuildVORewardRewardLeaguePromotion(string msgId)
        {
            RewardDlgVO vo = new RewardDlgVO("RewardLeaguePromotion");
            InboxMessage msg = inboxModel.items[msgId];

            vo.league = msg.league;
            AddRewardsToVO(vo, msg.rewards);

            return vo;
        }
        private RewardDlgVO BuildVORewardRewardTournamentEnd(string msgId)
        {
            RewardDlgVO vo = new RewardDlgVO("RewardTournamentEnd");
            InboxMessage msg = inboxModel.items[msgId];

            if (msg.tournamentType == TournamentConstants.TournamentType.MIN_1) vo.tournamentName = TournamentConstants.TournamentType.MIN_1;
            else if (msg.tournamentType == TournamentConstants.TournamentType.MIN_5) vo.tournamentName = TournamentConstants.TournamentType.MIN_5;
            else if (msg.tournamentType == TournamentConstants.TournamentType.MIN_10) vo.tournamentName = TournamentConstants.TournamentType.MIN_10;

            if (msg.chestType == TournamentConstants.ChestType.COMMON) vo.chestName = "Common Chest";
            if (msg.chestType == TournamentConstants.ChestType.EPIC) vo.chestName = "Epic Chest";
            if (msg.chestType == TournamentConstants.ChestType.RARE) vo.chestName = "Rare Chest";

            vo.trophiesCount = msg.trophiesCount;
            vo.rankCount = msg.rankCount;

            ChestIconsContainer container = ChestIconsContainer.Load();
            vo.chestImage = container.GetChest(msg.chestType);

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
