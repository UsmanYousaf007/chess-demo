/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class LobbyMediator : Mediator
    {
        // Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public DevFenValueChangedSignal devFenValueChangedSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public UpdateAdsSignal updateAdsSignal { get; set; }
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

        // View injection
        [Inject] public LobbyView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.playMultiplayerButtonClickedSignal.AddListener(OnPlayMultiplayerButtonClicked);
            view.playFriendsButtonClickedSignal.AddListener(OnPlayFriendsButtonClicked);
            view.playCPUButtonClickedSignal.AddListener(OnPlayCPUButtonClicked);
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked);

            view.storeItemClickedSignal.AddListener(OnStoreItemClicked);
            view.devFenValueChangedSignal.AddListener(OnDevFenValueChanged);
     
        }

        public override void OnRemove()
        {
            view.decStrengthButtonClickedSignal.RemoveAllListeners();
            view.incStrengthButtonClickedSignal.RemoveAllListeners();
            view.decDurationButtonClickedSignal.RemoveAllListeners();
            view.incDurationButtonClickedSignal.RemoveAllListeners();
            view.decPlayerColorButtonClickedSignal.RemoveAllListeners();
            view.incPlayerColorButtonClickedSignal.RemoveAllListeners();
            view.playMultiplayerButtonClickedSignal.RemoveAllListeners();
            view.playFriendsButtonClickedSignal.RemoveAllListeners();
            view.playCPUButtonClickedSignal.RemoveAllListeners();
            view.devFenValueChangedSignal.RemoveAllListeners();
            view.statsButtonClickedSignal.RemoveAllListeners();

            view.CleanUp();
        }
            
        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY) 
            {
                analyticsService.ScreenVisit(AnalyticsScreen.lobby);
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateMenuViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(LobbyVO vo)
        {
            view.UpdateStrength(vo);
        }

        [ListensTo(typeof(SetActionCountSignal))]
        public void OnSetActionCount(int count)
        {
            view.SetActionCount(count);
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

        private void OnPlayCPUButtonClicked()
        {
            startCPUGameSignal.Dispatch();
        }

        private void OnPlayMultiplayerButtonClicked()
        {
            findMatchSignal.Dispatch();
        }

        private void OnPlayFriendsButtonClicked()
        {
            loadFriendsSignal.Dispatch();
        }

        private void OnDevFenValueChanged(string fen)
        {
            devFenValueChangedSignal.Dispatch(fen);
        }
 
        private void OnUpdateAdsSignal()
        {
            updateAdsSignal.Dispatch();
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable, StoreVO vo)
        {
            if (isAvailable)
            {
                view.UpdateViewBundles(vo);
            }
        }

        [ListensTo(typeof(UpdatePurchasedBundleStoreItemSignal))]
        public void OnUpdatePurchasedBundleStoreItem(StoreVO vo, StoreItem item)
        { 
            view.HideBundles();
        }

        private void OnStoreItemClicked(StoreItem item)
        {
            // Purchase item after confirmation. No confirmation for remote store items
            purchaseStoreItemSignal.Dispatch(item.key, true);
        }
    }
}

