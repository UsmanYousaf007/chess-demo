using strange.extensions.command.impl;
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

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private const int TOTAL_PROMOTIONS = 5;
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
                showPromotionSignal.Dispatch(promotionCycle[promotionToShowIndex]);
            }
            else
            {
                var emptyPromotion = new PromotionVO
                {
                    cycleIndex = 0,
                    key = "none",
                    condition = null,
                    onClick = null                };

                showPromotionSignal.Dispatch(emptyPromotion);
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
                    string[] vServer = settingsModel.minimumClientVersion.Split('.');
                    string[] vClient = appInfoModel.clientVersion.Split('.');
                    

                    bool majorV = int.Parse(vServer[0]) > int.Parse(vClient[0]);
                    bool minorV = int.Parse(vServer[1]) > int.Parse(vClient[1]);

                    if (majorV == true && !isUpdateBannerShown)
                    {
                        return true;
                    }

                    if (minorV == true && !isUpdateBannerShown)
                    {
                        return true;
                    }

                    return isUpdateBannerShown;
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
                }
            };

            if(gameUpdateItem.condition())
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
            if (promotionCycle != null)
            {
                return;
            }

            promotionCycle = new List<PromotionVO>();

            var adsBanner = new PromotionVO
            {
                cycleIndex = 1,
                key = LobbyPromotionKeys.ADS_BANNER,
                condition = delegate
                {
                    return !(playerModel.HasSubscription() || playerModel.OwnsVGood(GSBackendKeys.ShopItem.REMOVE_ADS_PACK));
                },
                onClick = delegate
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.REMOVE_ADS_PACK, true);
                }
            };

            var lessonsBanner = new PromotionVO
            {
                cycleIndex = 2,
                key = LobbyPromotionKeys.LESSONS_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsAllLessons();
                },
                onClick = delegate 
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.ALL_LESSONS_PACK, true);
                }
            };

            var themesBanner = new PromotionVO
            {
                cycleIndex = 3,
                key = LobbyPromotionKeys.THEMES_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsAllThemes();
                },
                onClick = delegate 
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.ALL_THEMES_PACK, true);
                }
            };

            var subscriptionBanner = new PromotionVO
            {
                cycleIndex = 4,
                key = LobbyPromotionKeys.SUBSCRIPTION_BANNER,
                condition = delegate
                {
                    return !playerModel.HasSubscription();
                },
                onClick = delegate 
                {
                    audioService.PlayStandardClick();
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
                }
            };

            var rewardsBanner = new PromotionVO
            {
                cycleIndex = 5,
                key = LobbyPromotionKeys.REWARDS_BANNER,
                condition = delegate
                {
                    return !playerModel.HasSubscription();
                },
                onClick = delegate 
                {
                    audioService.PlayStandardClick();
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INVENTORY);
                }
            };

            promotionCycle.Add(adsBanner);
            promotionCycle.Add(lessonsBanner);
            promotionCycle.Add(themesBanner);
            promotionCycle.Add(subscriptionBanner);
            promotionCycle.Add(rewardsBanner);
        }

        IEnumerator LoadNextPromotionAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Execute();
        }
    }
}
