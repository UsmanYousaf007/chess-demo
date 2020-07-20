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
        [Inject] public ContactSupportSignal contactSupportSignal { get; set; }

        // Services
        [Inject] public IRateAppService rateAppService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.yesButton.onClick.AddListener(OnYesClicked);
            view.noButton.onClick.AddListener(OnNoClicked);

            view.closeButton.onClick.AddListener(OnNotNow);
            view.noDlgCloseButton.onClick.AddListener(OnNotNow);
            view.yesDlgCloseButton.onClick.AddListener(OnNotNow);
            view.noDlgMaybeButton.onClick.AddListener(OnNotNow);
            view.yesDlgMaybeButton.onClick.AddListener(OnNotNow);

            view.rateButton.onClick.AddListener(OnRate);

            view.tellUsButton.onClick.AddListener(OnTellUsBtnClick);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RATE_APP_DLG) 
            {
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
                preferencesModel.hasRated = true;
            }
        }

        private void OnNotNow()
        {
            rateAppService.RateApp(false);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnYesClicked()
        {
            view.ShowYesDialogue();
        }

        private void OnNoClicked()
        {
            view.ShowNoDialogue();
        }

        private void OnRate()
        {
            rateAppService.RateApp(true);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnTellUsBtnClick()
        {
            rateAppService.RateApp(false);
            audioService.PlayStandardClick();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            contactSupportSignal.Dispatch();
        }
    }
}
