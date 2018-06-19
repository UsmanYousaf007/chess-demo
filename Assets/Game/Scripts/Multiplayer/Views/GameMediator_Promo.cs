/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:07 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public PromoSelectedSignal promoSelectedSignal { get; set; }

        public void OnRegisterPromotions()
        {
            view.InitPromotions();
            view.promoClickedSignal.AddListener(OnPromoClicked);
        }

        public void OnRemovePromotions()
        {
            view.CleanupPromotions();
            view.promoClickedSignal.RemoveListener(OnPromoClicked);
        }

        /*
        [ListensTo(typeof(UpdatePromoDialogSignal))]
        public void OnUpdatePromoDialog(ChessColor color)
        {
            view.UpdatePromoDialog(color);
        }
        */

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowPromoDialog(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PROMO_DLG) 
            {
                view.ShowPromoDialog();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHidePromoDialog(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PROMO_DLG)
            {
                view.HidePromoDialog();
            }
        }

        private void OnPromoClicked(string pieceName)
        {
            promoSelectedSignal.Dispatch(pieceName);
        }
    }
}
