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
    public class NSSplash : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.SPLASH);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }
            else if (evt == NavigatorEvent.SHOW_CPU)
            {
                return new NSCPU();
            }
            else if (evt == NavigatorEvent.SHOW_UPDATE)
            {
                return new NSUpdate();
            }
            else if (evt == NavigatorEvent.SHOW_SKILL_LEVEL_DLG)
            {
                return new NSSkillLevelDlg();
            }
            else if (evt == NavigatorEvent.SHOW_MAINTENANCE_SCREEN)
            {
                return new NSMaintenance();
            }

            return null;
        }
    }
}
