/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 17:11:57 UTC+05:00

namespace TurboLabz.Gamebet
{
    public class NSMatchMaking : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.MATCH_MAKING);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_MP_PLAY)
            {
                return new NSMPPlay();
            }

            return null;
        }
    }
}