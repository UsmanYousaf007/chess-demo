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
using TurboLabz.Common;
using TurboLabz.CPUChess;
using System.Text;

namespace TurboLabz.Gamebet
{
    public class NavigatorCommand : Command
    {
        // Signal parameters
        [Inject] public NavigatorEvent navigatorEvent { get; set; }

        // Dispatch signals
        [Inject] public NavigatorShowViewSignal showViewSignal { get; set; }
        [Inject] public NavigatorHideViewSignal hideViewSignal { get; set; }
        [Inject] public LoadCPUMenuSignal loadCPUMenuSignal { get; set; }

        // Models
        [Inject] public INavigatorModel navigatorModel { get; set; }

        public override void Execute()
        {
            NS newState;

            if (navigatorModel.currentState == null)
            {
                LogUtil.Log("---- INITIALIZING NAVIGATOR ----", "green");
                navigatorModel.Reset();
                navigatorModel.currentState = new NSStart();
                navigatorModel.currentState.SetCommand(this);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Nav: " + navigatorModel.currentState.GetType().Name);
            sb.Append(" -> [" + navigatorEvent + "]");

            newState = navigatorModel.currentState.HandleEvent(navigatorEvent);

            if (newState != null)
            {
                newState.SetCommand(this);
                navigatorModel.previousState = navigatorModel.currentState;
                navigatorModel.currentState = newState;
                newState.RenderDisplayOnEnter();

                sb.Append(" -> " + newState.GetType().Name); 
            }
            else
            {
                sb.Append(" -> Ignored."); 
            }

            LogUtil.Log(sb.ToString(), "green");
        }
    }
}
