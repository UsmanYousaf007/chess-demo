/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-09 17:39:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{	
    public class ViewStateModel : IViewStateModel
    {
        public ViewId previousView { get; set; }
        public ViewId currentView { get; set; }

        public SubShopViewId subShopViewId { get; set;}
        public SubInventoryViewId subInventoryViewId { get; set;}
    }
}
