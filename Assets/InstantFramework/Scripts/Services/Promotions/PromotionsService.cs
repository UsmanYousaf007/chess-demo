/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using System.Collections;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class PromotionsService : IPromotionsService
    {
        public List<List<string>> promotionsSequence { get; set; }
        public bool promotionShown { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ActivePromotionSaleSingal activePromotionSaleSingal { get; set; }
        [Inject] public ShowFadeBlockerSignal showFadeBlockerSignal { get; set; }
        [Inject] public PromotionCycleOverSignal promotionCycleOverSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        private NormalRoutineRunner routineRunner = new NormalRoutineRunner();
        private Dictionary<string, PromoionDlgVO> promotionsMapping;

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            promotionsSequence = new List<List<string>>();
            promotionShown = false;
        }

        public void LoadRemoveAdsPromotion()
        {
            var promotionToShowKey = SelectSalePromotion(GSBackendKeys.ShopItem.SALE_REMOVE_ADS_PACK, GSBackendKeys.ShopItem.REMOVE_ADS_PACK);
            DispatchPromotion(promotionToShowKey);
        }

        public void LoadSubscriptionPromotion()
        {
            var promotionToShowKey = SelectSalePromotion("SubscriptionAnnualSale", "Subscription");
            DispatchPromotion(promotionToShowKey);
        }

        public bool IsSaleActive(string key)
        {
            return preferencesModel.activePromotionSales != null && preferencesModel.activePromotionSales.Contains(key);
        }

        public void LoadPromotion()
        {
            foreach (var sale in preferencesModel.activePromotionSales)
            {
                activePromotionSaleSingal.Dispatch(sale);
            }

            if (!promotionShown)
            {
                SelectAndDispatchPromotion();
            }
            else
            {
                OnPromotionCycleOver();
            }
        }

        private void SelectAndDispatchPromotion()
        {
            var wholeSequenceCheckedCount = 0;
            var sequence = GetSequence();
            SetupPromotions();

            //reset promotion index for old users in case they have seen all the promotions for the day
            preferencesModel.currentPromotionIndex = preferencesModel.currentPromotionIndex >= sequence.Count ? 0 : preferencesModel.currentPromotionIndex;

            //check for an active promotions in the sequence
            while (!promotionsMapping[sequence[preferencesModel.currentPromotionIndex]].condition())
            {
                preferencesModel.currentPromotionIndex++;

                //check from start again in case last promotion checked
                if (preferencesModel.currentPromotionIndex >= sequence.Count)
                {
                    preferencesModel.currentPromotionIndex = 0;
                    wholeSequenceCheckedCount++;

                    //this is to stop the infinite loop,
                    //it will loop through the sequence twice and breaks it non of the promotions are avaialble
                    if (wholeSequenceCheckedCount > 1)
                    {
                        OnPromotionCycleOver();
                        return;
                    }
                }
            }

            promotionShown = true;
            var promotionToDispatch = promotionsMapping[sequence[preferencesModel.currentPromotionIndex]];
            preferencesModel.currentPromotionIndex++;

            if (promotionToDispatch.key.Equals("Subscription"))
            {
                appInfoModel.isAutoSubscriptionDlgShown = true;
                subscriptionDlgClosedSignal.AddOnce(() => appInfoModel.isAutoSubscriptionDlgShown = false);
            }

            //navigatorEventSignal.Dispatch(promotionToDispatch.navigatorEvent);
            // showFadeBlockerSignal.Dispatch();
            routineRunner.StartCoroutine(DispatchPromotionCR(promotionToDispatch.navigatorEvent));

            if (promotionToDispatch.isOnSale)
            {
                preferencesModel.activePromotionSales.Add(promotionToDispatch.key);
                activePromotionSaleSingal.Dispatch(promotionToDispatch.key);
            }
        }

        private void OnPromotionCycleOver()
        {
            promotionCycleOverSignal.Dispatch();
        }

        private List<string> GetSequence()
        {
            var daysSincePlaying = (int)(TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp).ToLocalTime() - TimeUtil.ToDateTime(playerModel.creationDate).ToLocalTime()).TotalDays;
            var sequenceIndex = daysSincePlaying % promotionsSequence.Count;
            return promotionsSequence[sequenceIndex];
        }

        private string SelectSalePromotion(string saleKey, string originalKey)
        {
            return IsSaleActive(saleKey) ? saleKey : originalKey;
        }

        private void DispatchPromotion(string promotionKey)
        {
            SetupPromotions();

            if (promotionsMapping.ContainsKey(promotionKey) && promotionsMapping[promotionKey].condition())
            {
                navigatorEventSignal.Dispatch(promotionsMapping[promotionKey].navigatorEvent);
            }
        }

        private IEnumerator DispatchPromotionCR(NavigatorEvent eventId)
        {
            // Wait 2 frames so Lobby is ready before showing promo
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            navigatorEventSignal.Dispatch(eventId);
            showFadeBlockerSignal.Dispatch();
        }

        private void SetupPromotions()
        {
            if (promotionsMapping != null)
            {
                return;
            }

            var removeAdsFull = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.REMOVE_ADS_PACK,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_DLG,
                condition = delegate { return !playerModel.HasRemoveAds(); }
            };

            var lessons = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.ALL_LESSONS_PACK,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_CHESS_COURSE_DLG,
                condition = delegate { return !playerModel.OwnsAllLessons(); }
            };

            var themes = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.ALL_THEMES_PACK,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_CHESS_SETS_BUNDLE_DLG,
                condition = delegate { return !playerModel.OwnsAllThemes(); }
            };

            var subscription = new PromoionDlgVO
            {
                key = "Subscription",
                navigatorEvent = NavigatorEvent.SHOW_SUBSCRIPTION_DLG,
                condition = delegate { return !playerModel.HasSubscription(); }
            };

            var subscriptionSale = new PromoionDlgVO
            {
                key = "SubscriptionAnnualSale",
                navigatorEvent = NavigatorEvent.SHOW_SUBSCRIPTION_SALE_DLG,
                condition = delegate { return !playerModel.HasSubscription(); },
                isOnSale = true
            };

            var removeAdsSale = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.SALE_REMOVE_ADS_PACK,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_SALE_DLG,
                condition = delegate { return !playerModel.HasRemoveAds(); },
                isOnSale = true
            };

            var welcomeBundle = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_WELCOME_BUNDLE_DLG,
                condition = delegate { return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME); }
            };

            var eliteBundle = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_ELITE_BUNDLE_DLG,
                condition = delegate { return playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME); }
            };

            promotionsMapping = new Dictionary<string, PromoionDlgVO>();
            promotionsMapping.Add(removeAdsFull.key, removeAdsFull);
            promotionsMapping.Add(removeAdsSale.key, removeAdsSale);
            promotionsMapping.Add(subscription.key, subscription);
            promotionsMapping.Add(subscriptionSale.key, subscriptionSale);
            promotionsMapping.Add(welcomeBundle.key, welcomeBundle);
            promotionsMapping.Add(eliteBundle.key, eliteBundle);
            promotionsMapping.Add(lessons.key, lessons);
            promotionsMapping.Add(themes.key, themes);
        }
    }
}
