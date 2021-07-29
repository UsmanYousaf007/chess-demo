using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class RewardDlgV2Mediator : Mediator
    {
        //View Injection
        [Inject] public RewardDlgV2View view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        
        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        private RewardDlgV2VO _rewardDlgVO;
        private bool _rewardTrophies = false;

        public override void OnRegister()
        {
            view.Init();
            view.ContinueButtonSignal.AddListener(OnContinuePressed);
        }

        [ListensTo(typeof(UpdateRewardDlgV2ViewSignal))]
        public void UpdateView(RewardDlgV2VO vo)
        {
            _rewardDlgVO = vo;
            _rewardTrophies = _rewardDlgVO.TrophiesCount > 0;
            view.UpdateView(vo.Rewards[0], _rewardDlgVO.RVWatched, _rewardDlgVO.ShowChest);

            vo.Rewards.RemoveAt(0);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.REWARD_DLG_V2)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.REWARD_DLG_V2)
            {
                view.Hide();
            }
        }

        private void OnContinuePressed()
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            OnCollectAnimationComplete();
        }

        private void OnCollectAnimationComplete()
        {
            if (_rewardDlgVO.Rewards.Count > 0)
            {
                view.UpdateView(_rewardDlgVO.Rewards[0], _rewardDlgVO.RVWatched, _rewardDlgVO.ShowChest);
                _rewardDlgVO.Rewards.RemoveAt(0);
            }
            else
            {
                if (_rewardTrophies && _rewardDlgVO.TrophiesCount > 0)
                {
                    view.UpdateView(null, false, false, _rewardDlgVO.TrophiesCount);
                }
                else
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                }

                _rewardTrophies = false;
            }
        }
    }
}
