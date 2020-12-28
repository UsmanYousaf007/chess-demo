﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class SelectTimeModeMediator : Mediator
    {
        // View injection
        [Inject] public SelectTimeModeView view { get; set; }

        //Dispatch signals
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        //Listerners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        private MatchInfoVO matchInfoVO;
        private long betValue;

        public override void OnRegister()
        {
            view.Init();
            view.playMultiplayerButtonClickedSignal.AddListener(OnPlayMatch);
            view.powerModeButtonClickedSignal.AddListener(OnPowerModeButtonClicked);
            view.notEnoughCoinsSignal.AddListener(OnNotEnoughCoinsSignal);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGemsSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SELECT_TIME_MODE)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SELECT_TIME_MODE)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateTimeSelectDlgSignal))]
        public void UpdateView(long betValue)
        {
            this.betValue = betValue;
            view.UpdateView(betValue);
        }

        private void OnPlayMatch(string actionCode, bool isPowerMode)
        {
            matchInfoVO = new MatchInfoVO();
            matchInfoVO.actionCode = actionCode;
            matchInfoVO.betValue = betValue;
            matchInfoVO.powerMode = isPowerMode;

            var transactionVO = new VirtualGoodsTransactionVO();
            transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.COINS;
            transactionVO.consumeQuantity = (int)betValue;
            virtualGoodsTransactionResultSignal.AddOnce(OnTransactionResult);
            virtualGoodsTransactionSignal.Dispatch(transactionVO);
        }

        private void OnTransactionResult(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                FindMatchAction.Random(findMatchSignal, matchInfoVO);
            }
        }

        private void OnPowerModeButtonClicked()
        {
            purchaseStoreItemSignal.Dispatch(view.shortCode, true);
        }

        private void OnNotEnoughCoinsSignal()
        {

        }

        private void OnNotEnoughGemsSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.shortCode) && view.gameObject.activeInHierarchy)
            {
                view.OnEnablePowerMode();
            }
        }
    }
}
