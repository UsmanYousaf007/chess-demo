/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-23 19:05:50 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using TurboLabz.Gamebet;
using TurboLabz.Chess;

namespace TurboLabz.CPUChess
{
    public partial class GameMediator
    {
        public void OnRegisterMatchInfo()
        {
            view.InitMatchInfo();
        }
        
        [ListensTo(typeof(UpdateCPUGameInfoSignal))]
        public void OnUpdateCPUGameInfo(CPUGameInfoVO vo)
        {
            view.UpdateCPUGameInfo(vo);
        }
    }
}
