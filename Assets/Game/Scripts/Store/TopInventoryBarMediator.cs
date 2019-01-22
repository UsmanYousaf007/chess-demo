/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

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
        public void OnUpdateTopInventoryBar(PlayerInventoryVO vo)
        {
            view.UpdateTopInventoryBar(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            view.parentViewId = viewId;
        }


        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnUpdatePlayerInventory(PlayerInventoryVO vo)
        {
            view.UpdateTopInventoryBar(vo);
        }
    }
}
