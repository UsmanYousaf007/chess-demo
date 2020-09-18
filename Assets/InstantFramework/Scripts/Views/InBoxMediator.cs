﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class InboxMediator : Mediator
    {
        // View injection
        [Inject] public InboxView view { get; set; }

        [Inject] public IBackendService backendService { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadRewardDlgViewSignal loadRewardDlgViewSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardPartialSignal updateTournamentLeaderboardPartialSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getJoinedTournamentLeaderboardSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        public override void OnRegister()
        {
            view.Init();

            // Button click handlers
            view.inBoxBarClickedSignal.AddListener(OnInBoxBarClicked);
            view.bottoNavBackButtonClickedSignal.AddListener(OnBottomNavBackButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INBOX_VIEW)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.inbox);
                analyticsService.Event(AnalyticsEventId.inbox_visits);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INBOX_VIEW)
            {
                view.Hide();
            }
        }

        public void OnInBoxBarClicked(InboxBar inboxBar)
        {
            InboxMessage msg = inboxModel.items[inboxBar.msgId];
            if (msg.type == "RewardTournamentEnd" && msg.tournamentId != "unassigned")
            {
                // Create joined tournament here and add it to tournaments model joined list.
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament(msg.tournamentId);
                if (joinedTournament == null)
                {
                    JoinedTournamentData newJoinedTournament = new JoinedTournamentData();
                    newJoinedTournament.id = msg.tournamentId;
                    newJoinedTournament.type = msg.tournamentType;
                    newJoinedTournament.rank = msg.rankCount;
                    newJoinedTournament.ended = true;

                    tournamentsModel.joinedTournaments.Add(newJoinedTournament);
                    joinedTournament = newJoinedTournament;
                }

                updateTournamentLeaderboardPartialSignal.Dispatch(joinedTournament.id);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
                getJoinedTournamentLeaderboardSignal.Dispatch(joinedTournament.id, false);
            }
            else
            {
                loadRewardDlgViewSignal.Dispatch(inboxBar.msgId);
            }

            TLUtils.LogUtil.Log("InBoxMediator::OnInBoxBarClicked() ==>" + inboxBar.GetType().ToString());
        }

        public void OnBottomNavBackButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            audioService.PlayStandardClick();

            TLUtils.LogUtil.Log("InBoxMediator::OnBottomNavBackButtonClicked()");
        }

        [ListensTo(typeof(InboxAddMessagesSignal))]
        public void OnInboxAddMessages(Dictionary<string, InboxMessage> messages)
        {
            view.AddMessages(messages);
        }

        [ListensTo(typeof(InboxRemoveMessagesSignal))]
        public void OnInboxRemoveMessage(string messageId)
        {
            view.RemoveMessage(messageId);
        }

        [ListensTo(typeof(InboxFetchingMessagesSignal))]
        public void OnInboxFetchingMessages(bool isFetching)
        {
            view.processing.SetActive(isFetching);
        }
    }
}
