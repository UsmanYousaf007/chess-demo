/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-09 11:11:30 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;

namespace TurboLabz.InstantChess
{
    public class CPUGameModel : ICPUGameModel
    {
        public int cpuStrength { get; set; }
        public int durationIndex { get; set; }
        public int playerColorIndex { get; set; }
        public bool inProgress { get; set; }
        public string devFen { get; set; }

        [PostConstruct]
        public void LoadDefault()
        {
            Reset();
        }

        public void Reset()
        {
            cpuStrength = CPUSettings.DEFAULT_STRENGTH;
            durationIndex = CPUSettings.DEFAULT_TIMER_INDEX;
            playerColorIndex = CPUSettings.DEFAULT_PLAYER_COLOR_INDEX;
            inProgress = false;
            devFen = "";
        }

        public CPUMenuVO GetCPUMenuVO()
        {
            CPUMenuVO vo = new CPUMenuVO();
            vo.minStrength = CPUSettings.MIN_STRENGTH;
            vo.maxStrength = CPUSettings.MAX_STRENGTH;
            vo.selectedStrength = cpuStrength;
            vo.durationMinutes = CPUSettings.DURATION_MINUTES;
            vo.selectedDurationIndex = durationIndex;
            vo.playerColors = CPUSettings.PLAYER_COLORS;
            vo.selectedPlayerColorIndex = playerColorIndex;
            vo.inProgress = inProgress;

            return vo;
        }
    }
}
