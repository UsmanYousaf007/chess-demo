/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.InstantChess
{
    public class SkinRefsMediator : Mediator
    {
        // View injection
        [Inject] public SkinRefs view { get; set; }

        // Dispatch signals
        [Inject] public UpdateSkinSignal updateSkinSignal { get; set; }

        public override void OnRegister()
        {
            view.skinLoadedSignal.AddListener(OnSkinLoaded);
        }

        [ListensTo(typeof(ApplySkinSignal))]
        public void UpdateSkinSignal(string skinId)
        {
            view.ApplySkin(skinId);
        }

        private void OnSkinLoaded()
        {
            updateSkinSignal.Dispatch();
        }
    }
}