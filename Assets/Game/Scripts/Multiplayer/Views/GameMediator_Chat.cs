/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:01:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.MPChess 
{
    public partial class GameMediator
    {
        public void OnRegisterChat()
        {
            view.InitChat();
        }

        public void OnRemoveChat()
        {
            view.CleanupChat();
        }
    }
}
