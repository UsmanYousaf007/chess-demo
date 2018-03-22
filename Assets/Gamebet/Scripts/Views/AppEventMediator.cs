/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-21 13:53:37 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class AppEventMediator : Mediator
    {
        // Dispatch signals
        [Inject] public AppEventSignal appEventSignal { get; set; }

        // View injection
        [Inject] public AppEventView view { get; set; }

        public override void OnRegister()
        {
            view.appPausedSignal.AddListener(OnAppPaused);
            view.appResumedSignal.AddListener(OnAppResumed);
            view.appQuitSignal.AddListener(OnAppQuit);
            view.appEscapeSignal.AddListener(OnAppEscape);
        }

        public override void OnRemove()
        {
            view.appPausedSignal.RemoveListener(OnAppPaused);
            view.appResumedSignal.RemoveListener(OnAppResumed);
            view.appQuitSignal.RemoveListener(OnAppQuit);
        }

        private void OnAppPaused()
        {
            appEventSignal.Dispatch(AppEvent.PAUSED);
        }

        private void OnAppResumed()
        {
            appEventSignal.Dispatch(AppEvent.RESUMED);
        }

        private void OnAppQuit()
        {
            appEventSignal.Dispatch(AppEvent.QUIT);
        }

        private void OnAppEscape()
        {
            appEventSignal.Dispatch(AppEvent.ESCAPED);
        }
    }
}
