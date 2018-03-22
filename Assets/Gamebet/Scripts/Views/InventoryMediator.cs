/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-22 12:27:22 UTC+05:00

using strange.extensions.mediation.impl;

using TurboLabz.Common;
using strange.extensions.signal.impl;
using UnityEngine;

namespace TurboLabz.Gamebet
{
    public class InventoryMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadFreeCurrency1ModalSignal loadFreeCurrency1ModalSignal { get; set; }
        [Inject] public LoadInventoryChessSkinsInfoModalSignal loadInventoryChessSkinsInfoModalSignal { get; set; }
        [Inject] public ApplyPlayerInventorySignal applyPlayerInventorySignal { get; set; }

        [Inject] public LoadInventorySignal loadInventorySignal { get; set; } 
        [Inject] public LoadInventoryAvatarsSignal loadInventoryAvatarsSignal { get; set; } 
        [Inject] public LoadInventoryChessSkinsSignal loadInventoryChessSkinsSignal { get; set; }
        [Inject] public LoadInventoryLootSignal loadInventoryLootSignal { get; set; }
        [Inject] public LoadInventoryLootInfoModalSignal loadInventoryLootInfoModalSignal  { get; set; }
        [Inject] public LoadInventoryLootDismantleModalSignal loadInventoryLootDismantleModalSignal  { get; set; }
        [Inject] public ClaimLootBoxSignal claimLootBoxSignal { get; set; }
        [Inject] public AuthFacebookSignal authFacebookSignal { get; set; }

        public Signal claimLootBoxResultSignal = new Signal();

        // View injection
        [Inject] public InventoryView view { get; set; }

        public override void OnRegister()
        {
            view.currency1BuyButtonClickedSignal.AddListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.AddListener(OnCurrency2BuyButtonClicked);
            view.collectionClickedSignal.AddListener(OnCollectionButtonClicked);
            view.lootClickedSignal.AddListener(OnLootButtonClicked);
            view.avatarsButtonClickedSignal.AddListener(OnAvatarsButtonClicked);
            view.chessSkinsClickedSignal.AddListener(OnChessSkinsButtonClicked);
            view.chessSkinsThumbnailInfoButtonClickedSignal.AddListener(OnChessSkinsThumbnailInfoButtonClicked);
            view.backButtonClickedSignal.AddListener(OnBackButtonClicked);
            view.freeCurrency1ButtonClickedSignal.AddListener(OnFreeCurrency1ButtonClicked);
            view.menuPanelButtonClickedSignal.AddListener(OnMenuPanelButtonClicked);

            view.lootForgeCardInfoButtonClickedSignal.AddListener(OnForgeCardInfoButtonClicked);
            view.lootForgeCardDismantleButtonClickedSignal.AddListener(OnForgeCardDismantleButtonClicked);
            view.lootBoxThumbnailClickedSignal.AddListener(OnLootBoxThumbnailClicked);
            view.facebookAvatarLogInButtonClicked.AddListener(OnFacebookAvatarThumbnailButtonClicked);

            view.Init();
        }

        public override void OnRemove()
        {
            view.currency1BuyButtonClickedSignal.RemoveListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.RemoveListener(OnCurrency2BuyButtonClicked);
            view.lootClickedSignal.RemoveListener(OnLootButtonClicked);
            view.collectionClickedSignal.RemoveListener(OnCollectionButtonClicked);
            view.avatarsButtonClickedSignal.RemoveListener(OnAvatarsButtonClicked);
            view.chessSkinsClickedSignal.RemoveListener(OnChessSkinsButtonClicked);
            view.chessSkinsThumbnailInfoButtonClickedSignal.RemoveListener(OnChessSkinsThumbnailInfoButtonClicked);
            view.backButtonClickedSignal.RemoveListener(OnBackButtonClicked);
            view.freeCurrency1ButtonClickedSignal.RemoveListener(OnFreeCurrency1ButtonClicked);
            view.menuPanelButtonClickedSignal.RemoveListener(OnMenuPanelButtonClicked);

            view.lootForgeCardInfoButtonClickedSignal.RemoveListener(OnForgeCardInfoButtonClicked);
            view.lootForgeCardDismantleButtonClickedSignal.RemoveListener(OnForgeCardDismantleButtonClicked);
            view.lootBoxThumbnailClickedSignal.RemoveListener(OnLootBoxThumbnailClicked);
        }

        [ListensTo(typeof(UpdateInventoryViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdatePlayerProfilePictureSignal))]
        public void OnUpdateProfilePicture(Sprite sprite)
        {
            view.UpdateProfilePicture(sprite);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INVENTORY) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.INVENTORY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateInventoryLootViewSignal))]
        public void CreateLootScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateInventoryAvatarsViewSignal))]
        public void CreateAvatarsScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateInventoryChessSkinsViewSignal))]
        public void CreateChessSkinsScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        private void OnCurrency1BuyButtonClicked()
        {
            LogUtil.Log("Currency1Buy Button pressed" , "yellow");
        }

        private void OnCurrency2BuyButtonClicked()
        {
            LogUtil.Log("Currency2Buy Button pressed" , "yellow");
        }

        private void OnCollectionButtonClicked()
        {
            loadInventorySignal.Dispatch();
        }

        private void OnLootButtonClicked()
        {
            loadInventoryLootSignal.Dispatch();
        }

        private void OnFacebookAvatarThumbnailButtonClicked()
        {
            authFacebookSignal.Dispatch();
        }

        private void OnAvatarsButtonClicked()
        {
            loadInventoryAvatarsSignal.Dispatch();
        }

        private void OnChessSkinsButtonClicked()
        {
            loadInventoryChessSkinsSignal.Dispatch();
        }

        private void OnChessSkinsThumbnailInfoButtonClicked(string id, ShopVO vo)
        {
            vo.activeInventoryItemId = id;
            loadInventoryChessSkinsInfoModalSignal.Dispatch(vo);
            LogUtil.Log("I am mad modal", "red");
        }

        private void OnForgeCardInfoButtonClicked(string id, string forgeCardKey, ShopVO vo)
        {
            vo.activeInventoryItemId = id;
            vo.activeForgeCardItemId = forgeCardKey;
            loadInventoryLootInfoModalSignal.Dispatch(vo);
        }

        private void OnForgeCardDismantleButtonClicked(string id, string forgeCardKey, ShopVO vo)
        {
            vo.activeInventoryItemId = id;
            vo.activeForgeCardItemId = forgeCardKey;
            loadInventoryLootDismantleModalSignal.Dispatch(vo);
        }

        private void OnLootBoxThumbnailClicked(string purchasingId)
        {
            claimLootBoxSignal.Dispatch(purchasingId, claimLootBoxResultSignal);
        }

        private void OnBackButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.BACK_FROM_INVENTORY);
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            loadFreeCurrency1ModalSignal.Dispatch();
        }

        private void OnMenuPanelButtonClicked()
        {
            LogUtil.Log("Menu Button pressed" , "yellow");
        }
    }
}