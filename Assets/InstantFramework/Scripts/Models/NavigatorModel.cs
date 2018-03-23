/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class NavigatorModel : INavigatorModel
    {
        public NS currentState { get; set; }
        public NS previousState { get; set; }
        public List<NavigatorViewId> history { get; set; }
        public List<NavigatorViewId> viewStack { get; set; }

        public void Reset()
        {
            currentState = null;
            previousState = null;
            history = new List<NavigatorViewId>();
            viewStack = new List<NavigatorViewId>();
        }
    }
}
