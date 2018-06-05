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
        [Inject] public LoadMetaDataSignal loadMetaDataSignal  { get; set; }
        [Inject] public LoadMetaDataCompleteSignal loadMetaDataCompleteSignal { get; set; }

        // Models
        [Inject] public IMetaDataModel model { get; set; }

        public override void Execute()
        {
            Retain();
            loadMetaDataCompleteSignal.AddListener(OnLoadDataComplete);

            GSRequestSession.Instance.EndSession();
            loadMetaDataSignal.Dispatch();
            initBackendOnceSignal.Dispatch();
        }
            
        private void OnLoadDataComplete()
        {
            loadMetaDataCompleteSignal.RemoveListener(OnLoadDataComplete);

            // Check version information. Prompt the player if an update is needed.
            if (model.appInfo.appVersionValid == false)
            {
                // TODO: handle application update message
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE_APP);
                Release();
                return;
            }

            loadLobbySignal.Dispatch();
            Release();
        }

    }
}