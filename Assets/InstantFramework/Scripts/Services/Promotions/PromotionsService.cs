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

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAutoSubscriptionDailogueService autoSubscriptionDailogueService { get; set; }

        // Dispatch Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public ShowPromotionUpdateDlgSignal showPromotionUpdateDlgSignal { get; set; }

        private Dictionary<string, PromotionVO> promotionsMapping;

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            promotionsSequence = new List<List<string>>();
        }

        public void LoadPromotion(bool promotionShownThisSession)
        {
            if (Settings.ABTest.PROMOTION_TEST_GROUP == "E")
            {
                autoSubscriptionDailogueService.Show();
                return;
            }

            var showMultiplePromotionsPerSession = Settings.ABTest.PROMOTION_TEST_GROUP == "A" || Settings.ABTest.PROMOTION_TEST_GROUP == "B";

            if (showMultiplePromotionsPerSession || !promotionShownThisSession)
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
                return;
            }

            var promotionToDispatch = promotionsMapping[sequence[preferencesModel.currentPromotionIndex]];
            preferencesModel.currentPromotionIndex++;
            showPromotionUpdateDlgSignal.Dispatch(promotionToDispatch);
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
            var removeAdsFull = new PromotionVO
            {
                cycleIndex = 1,
                key = GSBackendKeys.ShopItem.REMOVE_ADS_PACK,
                analyticsContext = AnalyticsContext.lobby_remove_ads,
                condition = delegate
                {
                    return !playerModel.HasRemoveAds();
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.REMOVE_ADS_PACK, true);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_remove_ads);
                }
            };

            var lessons = new PromotionVO
            {
                cycleIndex = 2,
                key = GSBackendKeys.ShopItem.ALL_LESSONS_PACK,
                analyticsContext = AnalyticsContext.lobby_lessons_pack,
                condition = delegate
                {
                    return !playerModel.OwnsAllLessons();
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.ALL_LESSONS_PACK, true);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_lessons_pack);
                }
            };

            var themes = new PromotionVO
            {
                cycleIndex = 3,
                key = GSBackendKeys.ShopItem.ALL_THEMES_PACK,
                analyticsContext = AnalyticsContext.lobby_themes_pack,
                condition = delegate
                {
                    return !playerModel.OwnsAllThemes();
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.ALL_THEMES_PACK, true);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_themes_pack);
                }
            };

            var subscription = new PromotionVO
            {
                cycleIndex = 4,
                key = "Subscription",
                analyticsContext = AnalyticsContext.lobby_subscription_banner,
                condition = delegate
                {
                    return !playerModel.HasSubscription();
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_subscription_banner);
                }
            };

            var subscriptionSale = new PromotionVO
            {
                cycleIndex = 5,
                key = "SubscriptionAnnualSale",
                analyticsContext = AnalyticsContext.lobby_subscription_banner,
                condition = delegate
                {
                    return !playerModel.HasSubscription();
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_subscription_banner);
                }
            };

            var removeAdsSale = new PromotionVO
            {
                cycleIndex = 6,
                key = GSBackendKeys.ShopItem.SALE_REMOVE_ADS_PACK,
                analyticsContext = AnalyticsContext.lobby_remove_ads,
                condition = delegate
                {
                    return !playerModel.HasRemoveAds();
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.SALE_REMOVE_ADS_PACK, true);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_remove_ads);
                }
            };

            var welcomeBundle = new PromotionVO
            {
                cycleIndex = 7,
                key = GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME,
                analyticsContext = AnalyticsContext.lobby_remove_ads,
                condition = delegate
                {
                    return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME);
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME, true);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_remove_ads);
                }
            };

            var eliteBundle = new PromotionVO
            {
                cycleIndex = 8,
                key = GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE,
                analyticsContext = AnalyticsContext.lobby_remove_ads,
                condition = delegate
                {
                    return playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_WELCOME)
                    && (!playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE)
                    || (playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE) && DateTime.Now.DayOfWeek == DayOfWeek.Sunday));

                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ELITE, true);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_remove_ads);
                }
            };

            promotionsMapping = new Dictionary<string, PromotionVO>();
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
