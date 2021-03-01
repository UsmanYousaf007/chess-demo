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
        [Inject] public IAdsService adsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }

        // Signals
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public LoadCareerCardSignal loadCareerCardSignal { get; set; }
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }

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
                view.Show(playerModel.coins, playerModel.gems);
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
            view.UpdateView(vo, adsService.IsPersonalisedAdDlgShown());
        }

        public void OnCollectButtonClicked()
        {
            backendService.InBoxOpCollect(_dailyRewardVO.msgId).Then((res) => OnRewardCollected(false));
        }

        private void OnCollect2xButtonClicked()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_daily_reward);

            if (inboxModel.items.ContainsKey(_dailyRewardVO.msgId))
            {
                inboxModel.items.Remove(_dailyRewardVO.msgId);
            }
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_daily_reward)
            {
                switch (result)
                {
                    case AdsResult.FINISHED:
                        OnRewardCollected(true);
                        break;

                    //case AdsResult.FAILED: // Uncomment this for testing how tooltip is shown in inspectore
                    case AdsResult.NOT_AVAILABLE:
                        audioService.Play(audioService.sounds.SFX_TOOL_TIP);
                        view.toolTip.SetActive(true);
                        break;
                }
            }
        }

        private void OnRewardCollected(bool videoWatched)
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            _dailyRewardVO.onCloseSignal?.Dispatch();

            // Dispatch rewards sequence signal here
            if (videoWatched)
            {
                RewardDlgV2VO rewardDlgVO = new RewardDlgV2VO(_dailyRewardVO, videoWatched);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
                updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
            }

            updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            loadCareerCardSignal.Dispatch();
        }
    }
}
