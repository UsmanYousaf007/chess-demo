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

        public void LoadPromotion()
        {
            if (Settings.ABTest.PROMOTION_TEST_GROUP == "E")
            {
                promotionShown = false;
                return;
            }

            var showMultiplePromotionsPerSession = Settings.ABTest.PROMOTION_TEST_GROUP == "A" || Settings.ABTest.PROMOTION_TEST_GROUP == "B";

            if (showMultiplePromotionsPerSession || !promotionShown)
            {
                SelectAndDispatchPromotion();
            }
        }

        private void SelectAndDispatchPromotion()
        {
            if (promotionsMapping == null)
            {
                SetupPromotions();
            }

            var sequence = GetSequence();

            while (preferencesModel.currentPromotionIndex < sequence.Count && !promotionsMapping[sequence[preferencesModel.currentPromotionIndex]].condition())
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
        }

        private List<string> GetSequence()
        {
            var daysCycle = Settings.ABTest.PROMOTION_TEST_GROUP == "A" || Settings.ABTest.PROMOTION_TEST_GROUP == "B" ? 2 : 5;
            var daysSincePlaying = (DateTime.Now - TimeUtil.ToDateTime(playerModel.creationDate).ToLocalTime()).Days;
            var sequenceIndex = daysSincePlaying % daysCycle;
            //swap 0 and 1 in case test groups are B or D
            sequenceIndex = (Settings.ABTest.PROMOTION_TEST_GROUP == "B" || Settings.ABTest.PROMOTION_TEST_GROUP == "D") && sequenceIndex <= 1 ? 1 - sequenceIndex : sequenceIndex; 
            return promotionsSequence[sequenceIndex];
        }

        private void SetupPromotions()
        {
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
                condition = delegate { return !playerModel.HasSubscription(); }
            };

            var removeAdsSale = new PromoionDlgVO
            {
                key = GSBackendKeys.ShopItem.SALE_REMOVE_ADS_PACK,
                navigatorEvent = NavigatorEvent.SHOW_PROMOTION_REMOVE_ADS_SALE_DLG,
                condition = delegate { return !playerModel.HasRemoveAds(); }
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
