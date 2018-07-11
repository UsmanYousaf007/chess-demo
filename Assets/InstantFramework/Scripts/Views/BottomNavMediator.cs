/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class BottomNavMediator : Mediator
    {
        // View injection
        [Inject] public BottomNavView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
        [Inject] public LoadStoreSignal loadStoreSignal { get; set; }


        public override void OnRegister()
        {
            view.Init();

            view.homeButtonClickedSignal.AddListener(HomeButtonClicked);
            view.profileButtonClickedSignal.AddListener(ProfileButtonClicked);
            view.shopButtonClickedSignal.AddListener(ShopButtonClicked);
            view.settingsButtonClickedSignal.AddListener(SettingsButtonClicked);
        }

        public override void OnRemove()
        {
            view.homeButtonClickedSignal.RemoveAllListeners();
            view.profileButtonClickedSignal.RemoveAllListeners();
            view.shopButtonClickedSignal.RemoveAllListeners();
            view.settingsButtonClickedSignal.RemoveAllListeners();
        }

        void HomeButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }

        void ProfileButtonClicked()
        {
            loadStatsSignal.Dispatch();
        }

        void ShopButtonClicked()
        {
            loadStoreSignal.Dispatch();
        }

        void SettingsButtonClicked()
        {
            
        }
    }
}
