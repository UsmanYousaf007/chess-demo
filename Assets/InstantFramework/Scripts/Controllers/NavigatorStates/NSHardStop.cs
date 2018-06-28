/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-03-20 10:42:53 UTC+05:00

namespace TurboLabz.InstantFramework
{
    public class NSHardStop : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.HARD_STOP);  
        }
    }
}