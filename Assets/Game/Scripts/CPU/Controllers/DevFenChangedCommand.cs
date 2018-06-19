/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-17 14:45:12 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;

namespace TurboLabz.CPU
{
    public class DevFenChangedCommand : Command
    {
        // Parameters
        [Inject] public string devFen { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        public override void Execute()
        {
            cpuGameModel.devFen = devFen;
        }

    }
}
