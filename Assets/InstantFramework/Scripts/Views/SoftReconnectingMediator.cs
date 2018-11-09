/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class SoftReconnectingMediator : Mediator
    {
        // View injection
        [Inject] public SoftReconnectingView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(SoftReconnectingSignal))]
        public void OnShowView(bool isSoftReconnecting)
        {
            if (isSoftReconnecting)
            {
                view.Show();
            }
            else
            {
                view.Hide();
            }
        }
    }
}
