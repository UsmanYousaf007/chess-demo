/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-15 20:38:14 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantChess;
using System.Text;

namespace TurboLabz.InstantFramework
{
    public class NavigatorCommand : Command
    {
        // Signal parameters
        [Inject] public NavigatorEvent navigatorEvent { get; set; }

        // Dispatch signals
        [Inject] public NavigatorShowViewSignal showViewSignal { get; set; }
        [Inject] public NavigatorHideViewSignal hideViewSignal { get; set; }
        [Inject] public LoadCPUGameSignal loadGameSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        public override void Execute()
        {
            NS newState;

            if (navigatorModel.currentState == null)
            {
                navigatorModel.Reset();
                navigatorModel.currentState = new NSStart();
                navigatorModel.currentState.SetCommand(this);
            }

            newState = navigatorModel.currentState.HandleEvent(navigatorEvent);

            if (newState != null)
            {
                newState.SetCommand(this);
                navigatorModel.previousState = navigatorModel.currentState;
                navigatorModel.currentState = newState;
                newState.RenderDisplayOnEnter();
            }
        }
    }
}
