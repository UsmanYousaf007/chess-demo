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

        private const int TOTAL_PROMOTIONS = 1;
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
                analyticsService.Event(promotionCycle[promotionToShowIndex].analyticsImpId);
                showPromotionSignal.Dispatch(promotionCycle[promotionToShowIndex]);
            }
            else
            {
                var emptyPromotion = new PromotionVO
                {
                    cycleIndex = 0,
                    key = "none",
                    condition = null,
                    onClick = null,
                    analyticsImpId = 0
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
                condition = delegate
                {
                    return String.Compare(appInfoModel.clientVersion, settingsModel.minimumClientVersion) == -1 && !isUpdateBannerShown;
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
                },
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
            promotionCycle = new List<PromotionVO>();

            var ultimateItem = new PromotionVO
            {
                cycleIndex = 1,
                key = LobbyPromotionKeys.SUBSCRIPTION_BANNER,
                condition = delegate
                {
                    return !playerModel.HasSubscription();
                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    analyticsService.Event(AnalyticsEventId.tap_banner_subscription);
                    purchaseStoreItemSignal.Dispatch(key, true);
                },
                analyticsImpId = AnalyticsEventId.imp_banner_subscription
            };

            promotionCycle.Add(ultimateItem);
        }

        IEnumerator LoadNextPromotionAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Execute();
        }
    }
}
