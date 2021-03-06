/// @license Propriety <http://license.url>
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
    public class CareerCardMediator : Mediator
    {
        // View injection
        [Inject] public CareerCardView view { get; set; }

        //Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
        [Inject] public LoadTimeSelectDlgSignal loadTimeSelectDlgSignal { get; set; }
        [Inject] public LoadSpotCoinPurchaseSignal loadSpotCoinPurchaseSignal { get; set; }
        [Inject] public LoadRewardsSignal loadRewardsSignal { get; set; }

        //Listerners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private MatchInfoVO matchInfoVO;

        public override void OnRegister()
        {
            view.Init();
            view.OnPlayButtonClickedSignal.AddListener(PlayButtonClicked);
            view.OnInfoBtnClickedSignal.AddListener(InfoButtonClicked);
            view.notEnoughCoinsSignal.AddListener(OnNotEnoughCoinsSignal);
        }

        [ListensTo(typeof(UpdateCareerCardSignal))]
        public void UpdateView(CareerCardVO vo)
        {
            view.UpdateView(vo);
        }

        public void InfoButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LEAGUE_PERKS_VIEW);
        }

        public void PlayButtonClicked(long betValue)
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SELECT_TIME_MODE);
            loadTimeSelectDlgSignal.Dispatch(betValue);
            analyticsService.Event(AnalyticsEventId.bet_increment_used, AnalyticsParameter.context, betValue.ToString());
        }

        private void OnPlayMatch(string actionCode, long betValue)
        {
            matchInfoVO = new MatchInfoVO();
            matchInfoVO.actionCode = actionCode;
            matchInfoVO.betValue = betValue;

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
                FindMatchAction.Random(findMatchSignal, matchInfoVO, tournamentsModel.GetJoinedTournament().id);
            }
        }

        private void OnNotEnoughCoinsSignal(long betValue)
        {
            loadSpotCoinPurchaseSignal.Dispatch(betValue);
        }
    }
}
