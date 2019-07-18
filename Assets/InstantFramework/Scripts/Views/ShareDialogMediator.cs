/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework 
{
    public class ShareDialogMediator : Mediator
	{
        // Inject Services
        [Inject] public IScreenCaptureService screenCaptureService { get; set; }
        [Inject] public IShareService share { get; set; }


        //Inject Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ChessboardBlockerEnableSignal chessboardBlockerEnableSignal { get; set; }
        
        //Inject View
        [Inject] public ShareDialogView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.shareCloseButton.onClick.AddListener(OnCloseButtonClicked);
            view.shareButton.onClick.AddListener(OnShareButtonClicked);
        }

        public void OnCloseButtonClicked()
        {
            chessboardBlockerEnableSignal.Dispatch(false);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        public void OnShareButtonClicked()
        {
            share.ShareScreenShot("", "", Application.persistentDataPath + "/ShareScreenCapture", "");
            OnCloseButtonClicked();
        }

		[ListensTo(typeof(UpdateShareDialogSignal))]
		public void OnUpdateProfileDialog(Sprite sprite)
		{
            
            view.UpdateShareDialog(sprite);
		}

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHARE_SCREEN_DIALOG)
            {
                view.Show();
                screenCaptureService.CaptureScreenShot("ShareScreenCapture",1);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHARE_SCREEN_DIALOG)
            {
                view.Hide();
            }
        }

    }
}
