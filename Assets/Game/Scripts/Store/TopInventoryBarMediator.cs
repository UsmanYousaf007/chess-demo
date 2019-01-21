/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using strange.extensions.mediation.impl;

namespace TurboLabz.InstantGame
{
    public class TopInventoryBarMediator : Mediator
    {
        [Inject] public TopInventoryBarView view { get; set; }

        public override void OnRegister()
        {
            view.InitTopInventoryBar();
        }

        public override void OnRemove()
        {
            view.CleanupTopInventoryBar();
        }

        [ListensTo(typeof(UpdateTopInventoryBarSignal))]
        public void OnUpdateTopInventoryBar(TopInventoryBarVO vo)
        {
            view.UpdateTopInventoryBar(vo);
        }
    }
}
