using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ChampionshipNewRankDlgMediator : Mediator
    {
        //View Injection
        [Inject] public ChampionshipNewRankDlgView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getChampionshipTournamentLeaderboardSignal { get; set; }
        [Inject] public RankPromotedDlgClosedSignal rankPromotedDlgClosedSignal { get; set; }
        [Inject] public StartLobbyChampionshipTimerSignal startLobbyChampionshipTimerSignal { get; set; }
        [Inject] public GetProfilePictureSignal getProfilePictureSignal { get; set; }

        private int oldRank = -1;

        public override void OnRegister()
        {
            view.Init();
            view.loadPictureSignal.AddListener(OnLoadPicture);
            view.continueBtnClickedSignal.AddListener(OnContinuePressed);
        }

        private void OnLoadPicture(GetProfilePictureVO vo)
        {
            getProfilePictureSignal.Dispatch(vo);
        }

        // NOTE: Do not update on signal. New Rank dialog does not need to refresh when it is
        // already open.
        //[ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                view.UpdateView(playerModel.id, tournamentsModel.GetJoinedTournament());
            }
        }

        //[ListensTo(typeof(ResetTournamentsViewSignal))]
        //public void ResetView()
        //{
        //    view.ResetView();
        //}

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                view.Show(joinedTournament, oldRank != -1 && joinedTournament.rank > oldRank);
                oldRank = joinedTournament.rank;

                if (joinedTournament != null && joinedTournament.entries.Count > 0)
                {
                    view.UpdateView(playerModel.id, joinedTournament);
                }
                else
                {
                    getChampionshipTournamentLeaderboardSignal.Dispatch(joinedTournament.id, false);
                }

                view.UpdateLeagueTitle(playerModel, tournamentsModel);
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                view.Hide();
                view.ResetView();
                rankPromotedDlgClosedSignal.Dispatch();
            }
        }

        private void OnContinuePressed()
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            startLobbyChampionshipTimerSignal.Dispatch();
        }

        [ListensTo(typeof(ProfilePictureLoadedSignal))]
        public void OnPictureLoaded(string playerId, Sprite picture)
        {
            if (view.isActiveAndEnabled)
            {
                view.UpdatePicture(playerId, picture);
            }
        }

    }
}
