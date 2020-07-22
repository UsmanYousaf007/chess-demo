﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class NSLessonVideo : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.LESSON_VIDEO);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}

