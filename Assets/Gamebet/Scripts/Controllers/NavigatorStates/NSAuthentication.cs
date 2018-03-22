/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 11:10:39 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSAuthentication : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.AUTHENTICATION);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_LOADING)
            {
                return new NSLoading();
            }

            return null;
        }
    }
}