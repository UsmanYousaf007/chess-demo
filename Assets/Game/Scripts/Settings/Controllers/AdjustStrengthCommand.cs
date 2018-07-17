/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-12 11:06:03 UTC+05:00
///
/// @description
/// [add_description_here]

// .NET namespaces

// Unity namespaces

// Library namespaces
using strange.extensions.command.impl;

// User defined namespaces
using UnityEngine;
using TurboLabz.InstantFramework;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class AdjustStrengthCommand : Command
    {
        // Dispatch Signal
        [Inject] public UpdateStrengthSignal updateStrengthSignal { get; set; }
        [Inject] public SaveGameSignal saveGameSignal { get; set; }

        // Parameters
        [Inject] public bool increase { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            if (increase)
            {
                cpuGameModel.cpuStrength = Mathf.Min(CPUSettings.MAX_STRENGTH, cpuGameModel.cpuStrength + 1);
            }
            else
            {
                cpuGameModel.cpuStrength = Mathf.Max(CPUSettings.MIN_STRENGTH, cpuGameModel.cpuStrength - 1);
            }

			LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);
            updateStrengthSignal.Dispatch(vo);

            saveGameSignal.Dispatch(); // Save our changes to the prefs.
        }
    }
}
