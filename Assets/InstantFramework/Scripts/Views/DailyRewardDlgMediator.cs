using TurboLabz.InstantGame;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class DailyRewardDlgMediator : Mediator
    {
        //View Injection
        [Inject] public DailyRewardDlgView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        private Signal _onCloseSignal;

        public override void OnRegister()
        {
            view.Init();

            view._collectBtnClickedSignal.AddListener(OnCollectButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.DAILY_REWARD_DLG)
            {
                view.Show();
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.DAILY_REWARD_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateRewardDlgViewSignal))]
        public void OnUpdate(RewardDlgVO vo)
        {
            _onCloseSignal = vo.onCloseSignal;
            view.UpdateView(vo);
        }

        public void OnCollectButtonClicked(string msgId)
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            view.Hide();
            backendService.InBoxOpCollect(msgId);
            _onCloseSignal?.Dispatch();
        }
    }
}
