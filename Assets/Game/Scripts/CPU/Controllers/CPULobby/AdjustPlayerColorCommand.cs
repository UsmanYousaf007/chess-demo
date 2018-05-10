/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-12 12:14:54 UTC+05:00
///
/// @description
/// [add_description_here]

// .NET namespaces

// Unity namespaces

// Library namespaces
using strange.extensions.command.impl;

// User defined namespaces
using UnityEngine;

namespace TurboLabz.InstantChess
{
    public class AdjustPlayerColorCommand : Command
    {
        // Dispatch Signal
        [Inject] public UpdatePlayerColorSignal updatePlayerColorSignal { get; set; }

        // Parameters
        [Inject] public bool increase { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        public override void Execute()
        {
            if (increase)
            {
                cpuGameModel.playerColorIndex = Mathf.Min(CPUSettings.PLAYER_COLORS.Length - 1, cpuGameModel.playerColorIndex + 1);
            }
            else
            {
                cpuGameModel.playerColorIndex = Mathf.Max(0, cpuGameModel.playerColorIndex - 1);
            }

            updatePlayerColorSignal.Dispatch(cpuGameModel.GetCPUMenuVO());
        }
    }
}
