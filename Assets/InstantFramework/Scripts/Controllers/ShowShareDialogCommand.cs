using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class ShowShareDialogCommand : Command
    {
        //Navigation
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateShareDialogSignal updateShareDialogSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHARE_SCREEN_DLG);

            updateShareDialogSignal.Dispatch(null);
        }
    }
}