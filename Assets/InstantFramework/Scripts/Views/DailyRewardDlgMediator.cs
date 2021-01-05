using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class DailyRewardDlgMediator : Mediator
    {
        //View Injection
        [Inject] public DailyRewardDlgView view { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models

        // Signals
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }

        private RewardDlgVO _dailyRewardVO;

        public override void OnRegister()
        {
            view.Init();

            view._collectBtnClickedSignal.AddListener(OnCollectButtonClicked);
            view._collect2xBtnClickedSignal.AddListener(OnCollect2xButtonClicked);
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
            _dailyRewardVO = vo;
            view.UpdateView(vo);
        }

        public void OnCollectButtonClicked()
        {
            audioService.Play(audioService.sounds.SFX_CLICK);

            view.Hide();
            backendService.InBoxOpCollect(_dailyRewardVO.msgId);
            _dailyRewardVO.onCloseSignal?.Dispatch();

            // Dispatch rewards sequence signal here
            RewardDlgV2VO rewardDlgVO = CreateRewardDlgV2VO(false);
            updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
        }

        private void OnCollect2xButtonClicked()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_daily_reward);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_daily_reward)
            {
                switch (result)
                {
                    case AdsResult.FINISHED:
                        audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);

                        view.Hide();
                        _dailyRewardVO.onCloseSignal?.Dispatch();

                        // Dispatch rewards sequence signal here
                        RewardDlgV2VO rewardDlgVO = CreateRewardDlgV2VO(true);
                        updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
                        navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);

                        break;

                    //case AdsResult.FAILED: // Uncomment this for testing how tooltip is shown in inspectore
                    case AdsResult.NOT_AVAILABLE:
                        audioService.Play(audioService.sounds.SFX_TOOL_TIP);

                        view.toolTip.SetActive(true);
                        break;
                }
            }
        }

        private RewardDlgV2VO CreateRewardDlgV2VO(bool videoWatched)
        {
            RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO();
            rewardDlgVO.RVWatched = videoWatched;
            for (int i = 0; i < _dailyRewardVO.rewardShortCodes.Count; i++)
            {
                int quantity = _dailyRewardVO.GetRewardItemQty(i);
                if (videoWatched)
                {
                    quantity *= 2;
                }
                rewardDlgVO.Rewards.Add(new RewardDlgV2VO.Reward(_dailyRewardVO.rewardShortCodes[i], quantity));
            }

            return rewardDlgVO;
        }
    }
}
