/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-09 17:41:13 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
	public interface IViewStateModel
    {
        ViewId previousView { get; set; }
        ViewId currentView { get; set; }

        SubShopViewId subShopViewId { get; set;}
        SubInventoryViewId subInventoryViewId { get; set;}
	}
}
