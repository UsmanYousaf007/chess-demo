/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.mediation.impl;
using System.Collections.Generic;
using TurboLabz.Multiplayer;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System;

namespace TurboLabz.InstantFramework
{
    public class SettingsMediator : Mediator
    {
        // View injection
        [Inject] public SettingsView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }


        public override void OnRegister()
        {
            view.Init();

            view.restorePurchaseButtonClickedSignal.AddListener(OnRestorePurchases);
            view.upgradeToPremiumButtonClickedSignal.AddListener(OnUpgradeToPremiumClicked);
            view.backButton.onClick.AddListener(OnBackButtonClicked);
        }


        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SETTINGS)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.show_settings);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SETTINGS)
            {
                view.Hide();
            }
        }

        private void OnRestorePurchases()
        {
            restorePurchasesSignal.Dispatch();
        }

        void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                view.SetSubscriptionPrice();
            }
        }

        void OnUpgradeToPremiumClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
        }
    }
}

