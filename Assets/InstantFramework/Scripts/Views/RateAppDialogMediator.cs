/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class RateAppDialogMediator : Mediator
    {
        // Dispatch signals

        // View injection
        [Inject] public RateAppDialogView view { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IRateAppService rateAppService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.notNowButton.onClick.AddListener(OnNotNow);
            view.closeButton.onClick.AddListener(OnNotNow);
            view.rateButton.onClick.AddListener(OnRate);

            view.improveButton.onClick.AddListener(OnImproveBtnClick);
            view.likeButton.onClick.AddListener(OnLikeBtnClick);
            view.loveButton.onClick.AddListener(OnLoveBtnClick);
            view.tellUsButton.onClick.AddListener(OnTellUsBtnClick);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RATE_APP_DLG) 
            {
                rateAppService.RateApp(false);
                view.ShowAreYouEnjoying();
                analyticsService.ScreenVisit(AnalyticsScreen.rate_dialog);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RATE_APP_DLG)
            {
                view.Hide();
            }
        }

        private void OnNotNow()
        {
            rateAppService.RateApp(false);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            analyticsService.Event(AnalyticsEventId.tap_rate_no);
        }

        private void OnRate()
        {
            rateAppService.RateApp(true);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            analyticsService.Event(AnalyticsEventId.tap_rate_yes);
        }

        private void OnImproveBtnClick()
        {
            audioService.PlayStandardClick();
            view.ShowTellUs();
        }

        private void OnLikeBtnClick()
        {
            audioService.PlayStandardClick();
            view.ShowRateUs();
        }

        private void OnLoveBtnClick()
        {
            audioService.PlayStandardClick();
            view.ShowRateUs();
        }

        private void OnTellUsBtnClick()
        {
            audioService.PlayStandardClick();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            Application.OpenURL("mailto:" + Settings.SUPPORT_EMAIL);
            analyticsService.Event(AnalyticsEventId.tap_support);
        }
    }
}
