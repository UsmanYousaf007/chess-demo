/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-17 12:36:54 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        public void OnRegisterScore()
        {
            view.InitScore();   
        }

        public void OnRemoveScore()
        {
            view.CleanupScore();
        }
    }
}
