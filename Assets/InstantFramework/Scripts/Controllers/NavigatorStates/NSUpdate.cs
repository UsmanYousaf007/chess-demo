/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 10:43:07 UTC+05:00
namespace TurboLabz.InstantFramework
{
    public class NSUpdate : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.UPDATE);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            return null;
        }

    }
}