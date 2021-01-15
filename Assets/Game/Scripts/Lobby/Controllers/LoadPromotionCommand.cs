﻿using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using System;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
//using HUF.Analytics.API;
using IAnalyticsService = TurboLabz.InstantFramework.IAnalyticsService;

namespace TurboLabz.InstantGame
{
    public class LoadPromotionCommand : Command
    {
        //Singals
        [Inject] public ShowPromotionSignal showPromotionSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public UpdateSpotCoinsWatchAdDlgSignal updateSpotCoinsWatchAdDlgSignal { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        private const int TOTAL_PROMOTIONS = 4;
        private static List<PromotionVO> promotionCycle;
        private static bool isUpdateBannerShown;

        public override void Execute()
        {
            if (ShowGameUpdateBanner())
            {
                return;
            }

            if (!preferencesModel.isLobbyLoadedFirstTime)
            {
                return;
            }

            if (ShowCoinsBanner())
            {
                return;
            }

            Init();
            IncrementPromotionCycleIndex();

            int promotionToShowIndex = -1;
            int loopCount = 0;
            while (promotionToShowIndex == -1)
            {
                loopCount++;
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
                if (loopCount > 1)
                {
                    break;
                }
            }

            if (promotionToShowIndex != -1)
            {
                analyticsService.Event(AnalyticsEventId.banner_shown, promotionCycle[promotionToShowIndex].analyticsContext);
                showPromotionSignal.Dispatch(promotionCycle[promotionToShowIndex]);
            }
            else
            {
                var emptyPromotion = new PromotionVO
                {
                    cycleIndex = 0,
                    key = "none",
                    condition = null,
                    onClick = null
                };

                showPromotionSignal.Dispatch(emptyPromotion);
            }
        }

        private bool ShowGameUpdateBanner()
        {
            var gameUpdateItem = new PromotionVO
            {
                cycleIndex = 7,
                key = LobbyPromotionKeys.GAME_UPDATE_BANNER,
                analyticsContext = AnalyticsContext.lobby_update_banner,
                condition = delegate
                {
                    string[] vServer = settingsModel.minimumClientVersion.Split('.');
                    string[] vClient = appInfoModel.clientVersion.Split('.');
                    

                    bool majorV = int.Parse(vServer[0]) > int.Parse(vClient[0]);
                    bool minorV = int.Parse(vServer[1]) > int.Parse(vClient[1]);

                    return (majorV || minorV) && !isUpdateBannerShown;
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
#if UNITY_ANDROID
                    Application.OpenURL(appInfoModel.androidURL);
#elif UNITY_IOS
                    Application.OpenURL(appInfoModel.iosURL);
#else
                    LogUtil.Log("UPDATES NOT SUPPORTED ON THIS PLATFORM.", "red");
#endif
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_update_banner);
                }
            };

            if(gameUpdateItem.condition())
            {
                isUpdateBannerShown = true;
                analyticsService.Event(AnalyticsEventId.banner_shown, AnalyticsContext.lobby_update_banner);
                showPromotionSignal.Dispatch(gameUpdateItem);
                //routineRunner.StartCoroutine(LoadNextPromotionAfter(180f));
                return true;
            }

            return false;
        }

        private bool ShowCoinsBanner()
        {
            var coinsBanner = new PromotionVO
            {
                cycleIndex = 6,
                key = LobbyPromotionKeys.COINS_BANNER,
                analyticsContext = AnalyticsContext.lobby_out_of_coins,
                condition = delegate
                {
                    return playerModel.coins < settingsModel.bettingIncrements[0];
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_COIN_PURCHASE);
                    updateSpotCoinsWatchAdDlgSignal.Dispatch(0, storeSettingsModel.items["CoinPack1"], AdPlacements.Rewarded_coins_banner);
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_out_of_coins);
                }
            };

            if (coinsBanner.condition())
            {
                analyticsService.Event(AnalyticsEventId.banner_shown, AnalyticsContext.lobby_out_of_coins);
                showPromotionSignal.Dispatch(coinsBanner);
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
            if (promotionCycle != null)
            {
                return;
            }

            promotionCycle = new List<PromotionVO>();

            var adsBanner = new PromotionVO
            {
                cycleIndex = 1,
                key = LobbyPromotionKeys.ADS_BANNER,
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

            var lessonsBanner = new PromotionVO
            {
                cycleIndex = 2,
                key = LobbyPromotionKeys.LESSONS_BANNER,
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

            var themesBanner = new PromotionVO
            {
                cycleIndex = 3,
                key = LobbyPromotionKeys.THEMES_BANNER,
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

            var subscriptionBanner = new PromotionVO
            {
                cycleIndex = 4,
                key = LobbyPromotionKeys.SUBSCRIPTION_BANNER,
                analyticsContext = AnalyticsContext.lobby_subscription_banner,
                condition = delegate
                {
                    return !playerModel.HasSubscription();
                },
                onClick = delegate 
                {
                    audioService.PlayStandardClick();
                    promotionsService.LoadSubscriptionPromotion();
                    analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_subscription_banner);
                }
            };

            //var rewardsBanner = new PromotionVO
            //{
            //    cycleIndex = 5,
            //    key = LobbyPromotionKeys.REWARDS_BANNER,
            //    analyticsContext = AnalyticsContext.lobby_collect_rewards,
            //    condition = delegate
            //    {
            //        return true;
            //    },
            //    onClick = delegate 
            //    {
            //        audioService.PlayStandardClick();
            //        navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INVENTORY);
            //        analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.lobby_collect_rewards);
            //    }
            //};

            promotionCycle.Add(adsBanner);
            promotionCycle.Add(lessonsBanner);
            promotionCycle.Add(themesBanner);
            promotionCycle.Add(subscriptionBanner);
            //promotionCycle.Add(rewardsBanner);
        }

        IEnumerator LoadNextPromotionAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Execute();
        }
    }
}
