using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using System;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;

namespace TurboLabz.InstantGame
{
    public class LoadPromotionCommand : Command
    {
        //Singals
        [Inject] public ShowPromotionSignal showPromotionSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public LoadSpotPurchaseSignal loadSpotPurchaseSignal { get; set; }
        [Inject] public ShowCoachTrainingDailogueSignal showCoachTrainingDailogueSignal { get; set; }
        [Inject] public ShowStrengthTrainingDailogueSignal showStrengthTrainingDailogueSignal { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private const int TOTAL_PROMOTIONS = 6;
        private const int POWERUP_USE_LIMIT = 7;
        private const int POWERUP_TRAINING_DAYS_LIMIT = 30;
        private List<PromotionVO> promotionCycle;
        private static bool isUpdateBannerShown;

        public override void Execute()
        {
            if (ShowGameUpdateBanner())
            {
                return;
            }

            if (!preferencesModel.isLobbyLoadedFirstTime)
            {
                preferencesModel.timeAtLobbyLoadedFirstTime = DateTime.Now;
                return;
            }
            
            Init();
            IncrementPromotionCycleIndex();

            int promotionToShowIndex = -1;

            while (promotionToShowIndex == -1)
            {
                for (int i = 0; i < promotionCycle.Count; i++)
                {
                    if (preferencesModel.promotionCycleIndex.Equals(promotionCycle[i].cycleIndex))
                    {
                        if (promotionCycle[i].condition())
                        {
                            promotionToShowIndex = i;
                            break;
                        }
                        IncrementPromotionCycleIndex();
                    }
                }
            }

            if (promotionToShowIndex != -1)
            {
                analyticsService.Event(promotionCycle[promotionToShowIndex].analyticsImpId);
                showPromotionSignal.Dispatch(promotionCycle[promotionToShowIndex]);
            }
        }

        private bool ShowGameUpdateBanner()
        {
            var gameUpdateItem = new PromotionVO
            {
                cycleIndex = 7,
                key = LobbyPromotionKeys.GAME_UPDATE_BANNER,
                condition = delegate
                {
                    return String.Compare(appInfoModel.clientVersion, settingsModel.minimumClientVersion) == -1;
                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
#if UNITY_ANDROID
                    Application.OpenURL(appInfoModel.androidURL);
#elif UNITY_IOS
                    Application.OpenURL(appInfoModel.iosURL);
#else
                    LogUtil.Log("UPDATES NOT SUPPORTED ON THIS PLATFORM.", "red");
#endif
                    //analyticsService.Event(AnalyticsEventId.tap_banner_move_meter_training);
                },
                //analyticsImpId = AnalyticsEventId.imp_banner_move_meter_training
            };

            if(gameUpdateItem.condition() && !isUpdateBannerShown)
            {
                isUpdateBannerShown = true;
                showPromotionSignal.Dispatch(gameUpdateItem);
                routineRunner.StartCoroutine(LoadNextPromotionAfter(180f));
                return true;
            }

            return false;
        }

        private void IncrementPromotionCycleIndex()
        {
            preferencesModel.promotionCycleIndex++;
            preferencesModel.promotionCycleIndex = preferencesModel.promotionCycleIndex > TOTAL_PROMOTIONS ? 1 : preferencesModel.promotionCycleIndex;
        }

        private void Init()
        {
            promotionCycle = new List<PromotionVO>();

            var strengthItem = new PromotionVO
            {
                cycleIndex = 1,
                key = LobbyPromotionKeys.STRENGTH_BANNER,
                condition = delegate
                {
                    return preferencesModel.strengthUsedCount < POWERUP_USE_LIMIT
                    && (int)(DateTime.Now - preferencesModel.timeAtLobbyLoadedFirstTime).TotalDays < POWERUP_TRAINING_DAYS_LIMIT;
                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_move_meter_training);
                    showStrengthTrainingDailogueSignal.Dispatch();
                },
                analyticsImpId = AnalyticsEventId.imp_banner_move_meter_training
            };

            var coachItem = new PromotionVO
            {
                cycleIndex = 2,
                key = LobbyPromotionKeys.COACH_BANNER,
                condition = delegate
                {
                    return preferencesModel.coachUsedCount < POWERUP_USE_LIMIT
                    && (int)(DateTime.Now - preferencesModel.timeAtLobbyLoadedFirstTime).TotalDays < POWERUP_TRAINING_DAYS_LIMIT;

                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_coach_training);
                    showCoachTrainingDailogueSignal.Dispatch();
                },
                analyticsImpId = AnalyticsEventId.imp_banner_coach_training
            };

            var ultimateItem = new PromotionVO
            {
                cycleIndex = 3,
                key = LobbyPromotionKeys.ULTIMATE_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG);
                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_ultimate_bundle);
                    purchaseStoreItemSignal.Dispatch(key, true);
                },
                analyticsImpId = AnalyticsEventId.imp_banner_ultimate_bundle
            };

            var adsItem = new PromotionVO
            {
                cycleIndex = 4,
                key = LobbyPromotionKeys.ADS_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_NOADSFOREVER_SHOP_TAG)
                    && !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG);
                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_ad_bundle);
                    purchaseStoreItemSignal.Dispatch(key, true);
                },
                analyticsImpId = AnalyticsEventId.imp_banner_ad_bundle
            };
            
            var strengthPurchase = new PromotionVO
            {
                cycleIndex = 5,
                key = LobbyPromotionKeys.STRENGTH_PURCHASE,
                condition = delegate
                {
                    return preferencesModel.strengthUsedCount >= POWERUP_USE_LIMIT
                    || (int)(DateTime.Now - preferencesModel.timeAtLobbyLoadedFirstTime).TotalDays >= POWERUP_TRAINING_DAYS_LIMIT;

                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_move_meter_purchase);
                    loadSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.MOVEMETER);
                },
                analyticsImpId = AnalyticsEventId.imp_banner_move_meter_purchase
            };

            var coachPurchase = new PromotionVO
            {
                cycleIndex = 6,
                key = LobbyPromotionKeys.COACH_PURCHASE,
                condition = delegate
                {
                    return preferencesModel.coachUsedCount >= POWERUP_USE_LIMIT
                    || (int)(DateTime.Now - preferencesModel.timeAtLobbyLoadedFirstTime).TotalDays >= POWERUP_TRAINING_DAYS_LIMIT;

                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_coach_purchase);
                    loadSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.COACH);
                },
                analyticsImpId = AnalyticsEventId.imp_banner_coach_purchase
            };

            promotionCycle.Add(strengthItem);
            promotionCycle.Add(coachItem);
            promotionCycle.Add(ultimateItem);
            promotionCycle.Add(adsItem);
            promotionCycle.Add(strengthPurchase);
            promotionCycle.Add(coachPurchase);

            promotionCycle.Sort((x,y) => x.cycleIndex.CompareTo(y.cycleIndex));
        }

        IEnumerator LoadNextPromotionAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Execute();
        }
    }
}
