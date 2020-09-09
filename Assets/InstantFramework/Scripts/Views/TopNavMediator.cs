/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class TopNavMediator : Mediator
    {
        // View injection
        [Inject] public TopNavView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ContactSupportSignal contactSupportSignal { get; set; }
        [Inject] public LoadInboxSignal loadInboxSignal { get; set; }

        //Services
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.settingsButtonClickedSignal.AddListener(OnSettingsButtonClicked);
            view.supportButtonClicked.AddListener(OnSupportButtonClicked);
            view.addGemsButtonClickedSignal.AddListener(OnAddGemsButtonClicked);
            view.inboxButtonClickedSignal.AddListener(OnInboxButtonClicked);
        }

        public override void OnRemove()
        {
            view.settingsButtonClickedSignal.RemoveAllListeners();
            view.supportButtonClicked.RemoveAllListeners();
            view.addGemsButtonClickedSignal.RemoveAllListeners();
        }

        private void OnSettingsButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SETTINGS);
            hAnalyticsService.LogEvent("clicked", "menu", "", "settings");
        }

        private void OnSupportButtonClicked()
        {
            hAnalyticsService.LogEvent("clicked", "menu", "", "support");
            contactSupportSignal.Dispatch();
        }

        private void OnAddGemsButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP);
        }

        private void OnInboxButtonClicked()
        {
            loadInboxSignal.Dispatch();
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnGemsUpdated(PlayerInventoryVO inventory)
        {
            view.UpdateGemsCount(inventory.gemsCount);
        }

        [ListensTo(typeof(UpdateInboxMessageCountViewSignal))]
        public void OnMessagesUpdated(long messagesCount)
        {
            view.UpdateMessagesCount(messagesCount);
        }
    }
}
