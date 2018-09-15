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
        [Inject] public GetInitDataSignal getInitDataSignal  { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }


        // Models
        [Inject] public IMetaDataModel model { get; set; }

        public override void Execute()
        {
            CommandBegin();

            getInitDataSignal.Dispatch();

        }
            
        private void OnGetInitDataComplete()
        {
            // Check version information. Prompt the player if an update is needed.
            if (model.appInfo.appBackendVersionValid == false)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE);
                CommandEnd();
                return;
            }

            initBackendOnceSignal.Dispatch();
            loadLobbySignal.Dispatch();

            if (facebookService.isLoggedIn())
            {
                refreshFriendsSignal.Dispatch();
                refreshCommunitySignal.Dispatch();
            }
                
            CommandEnd();
        }

        private void CommandBegin()
        {
            Retain();
            getInitDataCompleteSignal.AddListener(OnGetInitDataComplete);
        }

        private void CommandEnd()
        {
            getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
            Release();
        }

    }
}