/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class NavigatorModel : INavigatorModel
    {
        public NS currentState { get; set; }
        public NS previousState { get; set; }
        public List<NavigatorViewId> history { get; set; }
        public List<NavigatorViewId> viewStack { get; set; }
        public NavigatorEvent ignoreEvent { get; set; }
        public NavigatorViewId currentViewId
        {
            get
            {
                return viewStack.Count == 0 ? NavigatorViewId.NONE : viewStack[viewStack.Count - 1];
            }
        }

        /*
         * This is called only once by the NavigatorCommand and should not be called
         * again for the lifetime of the app including reconnects.
         */
        public void Reset()
        {
            currentState = null;
            previousState = null;
            history = new List<NavigatorViewId>();
            viewStack = new List<NavigatorViewId>();
            ignoreEvent = NavigatorEvent.NONE;
        }
    }
}
