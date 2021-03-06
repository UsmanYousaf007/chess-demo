/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        [Inject] public ShowShareScreenDialogSignal shareScreenDialogSignal { get; set; }

        [Inject] public IScreenCaptureService screenCaptureService { get; set; }

        public void OnRegisterBotBar()
        {
            view.InitBotBar();
            view.backToLobbySignal.AddListener(OnExitBackToLobby);
            view.shareScreenButton.onClick.AddListener(OnShareScreenClicked);
            view.ShowShareDialogSignal.AddListener(OnShowShareDialogSignal);
            view.analysisExitButtonClickedSignal.AddListener(OnAnalysisExitButtonSignal);
        }

        public void OnExitBackToLobby()
        {
            exitLongMatchSignal.Dispatch();
        }

        public void OnShareScreenClicked()
        {
            screenCaptureService.CaptureScreenShot(view.logoObject.activeSelf ? null : view.logo);
            view.OpenShareDialog();
        }

        public void OnShowShareDialogSignal()
        {
            shareScreenDialogSignal.Dispatch();
        }

        private void OnAnalysisExitButtonSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
