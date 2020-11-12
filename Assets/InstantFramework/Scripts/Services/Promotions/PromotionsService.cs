/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class PromotionsService : IPromotionsService
    {
        public List<List<string>> promotionsSequence { get; set; }
        public bool promotionShown { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ActivePromotionSaleSingal activePromotionSaleSingal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

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

            if (Settings.ABTest.PROMOTION_TEST_GROUP == "E")
            {
                promotionShown = false;
                return;
            }

            var promotionTestGroup = GetPromotionTestGroup();
            var showMultiplePromotionsPerSession = promotionTestGroup == "A" || promotionTestGroup == "B";

            if (showMultiplePromotionsPerSession || !promotionShown)
            {
                SelectAndDispatchPromotion();
            }
        }

        private void SelectAndDispatchPromotion()
        {
            var sequence = GetSequence();
            SetupPromotions();

            while (preferencesModel.currentPromotionIndex < sequence.Count
               && !promotionsMapping[sequence[preferencesModel.currentPromotionIndex]].condition())
            {
                preferencesModel.currentPromotionIndex++;
            }

            if (preferencesModel.currentPromotionIndex >= sequence.Count)
            {
                promotionShown = false;
                return;
            }

            promotionShown = true;
            var promotionToDispatch = promotionsMapping[sequence[preferencesModel.currentPromotionIndex]];
            preferencesModel.currentPromotionIndex++;
            navigatorEventSignal.Dispatch(promotionToDispatch.navigatorEvent);

            if (promotionToDispatch.isOnSale)
            {
                preferencesModel.activePromotionSales.Add(promotionToDispatch.key);
                activePromotionSaleSingal.Dispatch(promotionToDispatch.key);
            }
        }

        private List<string> GetSequence()
        {
            var promotionTestGroup = GetPromotionTestGroup();
            var daysCycle = promotionTestGroup == "A" || promotionTestGroup == "B" ? 2 : 5;
            var daysSincePlaying = (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp).ToLocalTime() - preferencesModel.lastLaunchTime).Days;
            var sequenceIndex = daysSincePlaying % daysCycle;
            //swap 0 and 1 in case test groups are B or D
            sequenceIndex = (promotionTestGroup == "B" || promotionTestGroup == "D") && sequenceIndex <= 1 ? 1 - sequenceIndex : sequenceIndex; 
            return promotionsSequence[sequenceIndex];
        }

        private string SelectSalePromotion(string saleKey, string originalKey)
        {
            return  IsSaleActive(saleKey) ? saleKey : originalKey;
        }

        private void DispatchPromotion(string promotionKey)
        {
            SetupPromotions();

            if (promotionsMapping.ContainsKey(promotionKey) && promotionsMapping[promotionKey].condition())
            {
                navigatorEventSignal.Dispatch(promotionsMapping[promotionKey].navigatorEvent);
            }
        }

        private string GetPromotionTestGroup()
        {
            return Settings.ABTest.PROMOTION_TEST_GROUP.ToUpper();
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
                condition = delegate
                {
                    return playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME) 
                    && (!playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE) || DateTime.Now.DayOfWeek == DayOfWeek.Sunday);

                }
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
