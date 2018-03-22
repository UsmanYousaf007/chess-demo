/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-31 03:15:36 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class ShopMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadFreeCurrency1ModalSignal loadFreeCurrency1ModalSignal { get; set; }
        [Inject] public LoadShopLootBoxesModalSignal loadShopLootBoxesModalSignal { get; set; } 
        [Inject] public LoadShopAvatarsModalSignal loadShopAvatarsModalSignal { get; set; } 
        [Inject] public LoadShopAvatarsBorderModalSignal loadShopAvatarsBorderModalSignal { get; set; } 
        [Inject] public LoadShopChessSkinsModalSignal loadShopChessSkinsModalSignal { get; set; } 
        [Inject] public LoadShopCurrency1ModalSignal loadShopCurrency1ModalSignal { get; set; } 
        [Inject] public LoadShopCurrency2ModalSignal loadShopCurrency2ModalSignal { get; set; } 

        [Inject] public LoadShopLootBoxesSignal loadShopLootBoxesSignal { get; set; } 
        [Inject] public LoadShopAvatarsSignal loadShopAvatarsSignal { get; set; } 
        [Inject] public LoadShopChessSkinsSignal loadShopChessSkinsSignal { get; set; } 
        [Inject] public LoadShopChatSignal loadShopChatSignal { get; set; } 
        [Inject] public LoadShopCurrencySignal loadShopCurrencySignal { get; set; } 

        // View injection
        [Inject] public ShopView view { get; set; }
        
        public override void OnRegister()
        {
            view.currency1BuyButtonClickedSignal.AddListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.AddListener(OnCurrency2BuyButtonClicked);
            view.lootBoxesButtonClickedSignal.AddListener(OnLootBoxesButtonClicked);
            view.avatarsButtonClickedSignal.AddListener(OnAvatarsButtonClicked);
            view.chessSkinsClickedSignal.AddListener(OnChessSkinsButtonClicked);
            view.chatButtonClickedSignal.AddListener(OnChatButtonClicked);
            view.currencyButtonClickedSignal.AddListener(OnCurrencyButtonClicked);
            view.backButtonClickedSignal.AddListener(OnBackButtonClicked);
            view.freeCurrency1ButtonClickedSignal.AddListener(OnFreeCurrency1ButtonClicked);
            view.menuPanelButtonClickedSignal.AddListener(OnMenuPanelButtonClicked);

            view.lootBoxesThumbnailClickedSignal.AddListener(OnLootBoxesThumbnailButtonClicked);
            view.avatarsThumbnailClickedSignal.AddListener(OnAvatarsThumbnailButtonClicked);
            view.avatarsBorderThumbnailClickedSignal.AddListener(OnAvatarsBorderThumbnailButtonClicked);
            view.chessSkinThumbnailClickedSignal.AddListener(OnChessSkinsThumbnailButtonClicked);
            view.currency1ThumbnailClickedSignal.AddListener(OnCurrency1ThumbnailButtonClicked);
            view.currency2ThumbnailClickedSignal.AddListener(OnCurrency2ThumbnailButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.currency1BuyButtonClickedSignal.RemoveListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.RemoveListener(OnCurrency2BuyButtonClicked);
            view.lootBoxesButtonClickedSignal.RemoveListener(OnLootBoxesButtonClicked);
            view.avatarsButtonClickedSignal.RemoveListener(OnAvatarsButtonClicked);
            view.chessSkinsClickedSignal.RemoveListener(OnChessSkinsButtonClicked);
            view.chatButtonClickedSignal.RemoveListener(OnChatButtonClicked);
            view.currencyButtonClickedSignal.RemoveListener(OnCurrencyButtonClicked);
            view.backButtonClickedSignal.RemoveListener(OnBackButtonClicked);
            view.freeCurrency1ButtonClickedSignal.RemoveListener(OnFreeCurrency1ButtonClicked);
            view.menuPanelButtonClickedSignal.RemoveListener(OnMenuPanelButtonClicked);

            view.lootBoxesThumbnailClickedSignal.RemoveListener(OnLootBoxesThumbnailButtonClicked);
            view.avatarsThumbnailClickedSignal.RemoveListener(OnAvatarsThumbnailButtonClicked);
            view.avatarsBorderThumbnailClickedSignal.RemoveListener(OnAvatarsBorderThumbnailButtonClicked);
            view.chessSkinThumbnailClickedSignal.RemoveListener(OnChessSkinsThumbnailButtonClicked);
            view.currency1ThumbnailClickedSignal.RemoveListener(OnCurrency1ThumbnailButtonClicked);
            view.currency2ThumbnailClickedSignal.RemoveListener(OnCurrency2ThumbnailButtonClicked);
        }

        [ListensTo(typeof(UpdateShopViewSignal))]
        public void OnUpdateView(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP || viewId == NavigatorViewId.SHOP_LOOT_BOXES ||
                viewId == NavigatorViewId.SHOP_AVATARS|| viewId == NavigatorViewId.SHOP_CHESS_SKINS ||
                viewId == NavigatorViewId.SHOP_CHAT || viewId == NavigatorViewId.SHOP_CURRENCY) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP || viewId == NavigatorViewId.SHOP_LOOT_BOXES ||
                viewId == NavigatorViewId.SHOP_AVATARS|| viewId == NavigatorViewId.SHOP_CHESS_SKINS ||
                viewId == NavigatorViewId.SHOP_CHAT || viewId == NavigatorViewId.SHOP_CURRENCY)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateShopLootBoxesViewSignal))]
        public void CreateLootBoxesScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateShopAvatarsViewSignal))]
        public void CreateAvatarsScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateShopChessSkinsViewSignal))]
        public void CreateChessSkinsScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateShopChatViewSignal))]
        public void CreateChatScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateShopCurrencyViewSignal))]
        public void CreateCurrencyScreen(ShopVO vo)
        {
            view.UpdateView(vo);
        }
          
        private void OnCurrency1BuyButtonClicked()
        {
            loadShopCurrencySignal.Dispatch();
        }

        private void OnCurrency2BuyButtonClicked()
        {
            loadShopCurrencySignal.Dispatch();
        }

        private void OnLootBoxesButtonClicked()
        {
            loadShopLootBoxesSignal.Dispatch();
        }

        private void OnAvatarsButtonClicked()
        {
            loadShopAvatarsSignal.Dispatch();
        }

        private void OnChessSkinsButtonClicked()
        {
            loadShopChessSkinsSignal.Dispatch();
        }

        private void OnChatButtonClicked()
        {
            loadShopChatSignal.Dispatch();
        }

        private void OnCurrencyButtonClicked()
        {
            loadShopCurrencySignal.Dispatch();
        }

        private void OnBackButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.BACK_FROM_SHOP);
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            loadFreeCurrency1ModalSignal.Dispatch();
        }

        private void OnMenuPanelButtonClicked()
        {
            LogUtil.Log("Menu Button pressed" , "yellow");
        }

        private void OnLootBoxesThumbnailButtonClicked(string id, ShopVO vo)
        {
            vo.activeShopItemId = id;
            loadShopLootBoxesModalSignal.Dispatch(vo);
        }

        private void OnAvatarsThumbnailButtonClicked(string id, ShopVO vo)
        {
            vo.activeShopItemId = id;
            loadShopAvatarsModalSignal.Dispatch(vo);
        }

        private void OnAvatarsBorderThumbnailButtonClicked(string id, ShopVO vo)
        {
            vo.activeShopItemId = id;
            loadShopAvatarsBorderModalSignal.Dispatch(vo);
        }

        private void OnChessSkinsThumbnailButtonClicked(string id, ShopVO vo)
        {
            vo.activeShopItemId = id;
            loadShopChessSkinsModalSignal.Dispatch(vo);
        }

        private void OnCurrency1ThumbnailButtonClicked(string id, ShopVO vo)
        {
            vo.activeShopItemId = id;
            loadShopCurrency1ModalSignal.Dispatch(vo);
        }

        private void OnCurrency2ThumbnailButtonClicked(string id, ShopVO vo)
        {
            vo.activeShopItemId = id;
            loadShopCurrency2ModalSignal.Dispatch(vo);
        }
    }
}