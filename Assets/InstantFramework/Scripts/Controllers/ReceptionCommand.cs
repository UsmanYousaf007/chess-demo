/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ReceptionCommand : Command
    {
        // Dispatch signals
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public InitBackendOnceSignal initBackendOnceSignal { get; set; }
        [Inject] public InitGameDataSignal loadGameDataSignal  { get; set; }
        [Inject] public LoadMetaDataCompleteSignal loadMetaDataCompleteSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IMetaDataModel model { get; set; }

        public override void Execute()
        {
            CommandBegin();

            loadGameDataSignal.Dispatch();
            initBackendOnceSignal.Dispatch();
        }
            
        private void OnLoadDataComplete()
        {
            // Check version information. Prompt the player if an update is needed.
            if (model.appInfo.appBackendVersionValid == false)
            {
                // TODO: handle application update message
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE);



                CommandEnd();
                return;
            }
                
            loadLobbySignal.Dispatch();
            CommandEnd();
        }

        private void CommandBegin()
        {
            Retain();
            loadMetaDataCompleteSignal.AddListener(OnLoadDataComplete);
        }

        private void CommandEnd()
        {
            loadMetaDataCompleteSignal.RemoveListener(OnLoadDataComplete);
            Release();
        }

    }
}