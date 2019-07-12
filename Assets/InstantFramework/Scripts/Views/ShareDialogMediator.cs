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
        [Inject] public ShareDialogView view { get; set; }

        public void OnShareButtonClicked()
        {
            
        }

		[ListensTo(typeof(UpdateShareDialogSignal))]
		public void OnUpdateProfileDialog(Sprite sprite)
		{
            view.UpdateShareDialog(sprite);
		}

        [ListensTo(typeof(ShowShareScreenDialogSignal))]
        public void ShowShareScreen()
        {
            view.ShowShareDialog();
        }
    }
}
