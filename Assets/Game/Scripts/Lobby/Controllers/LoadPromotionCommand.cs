using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using System;

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

        //Services
        [Inject] public IAudioService audioService { get; set; }

        private const int TOTAL_PROMOTIONS = 6;
        private const int POWERUP_USE_LIMIT = 7;
        private const int POWERUP_TRAINING_DAYS_LIMIT = 30;
        private List<PromotionVO> promotionCycle;

        public override void Execute()
        {
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
                showPromotionSignal.Dispatch(promotionCycle[promotionToShowIndex]);
            }
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
                    showStrengthTrainingDailogueSignal.Dispatch();
                }
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
                    showCoachTrainingDailogueSignal.Dispatch();
                }
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
                    purchaseStoreItemSignal.Dispatch(key, true);
                }
            };

            var adsItem = new PromotionVO
            {
                cycleIndex = 4,
                key = LobbyPromotionKeys.ADS_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_NOADSFOREVER_SHOP_TAG);
                },
                onClick = delegate (string key)
                {
                    audioService.PlayStandardClick();
                    purchaseStoreItemSignal.Dispatch(key, true);
                }
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
                    loadSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.MOVEMETER);
                }
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
                    loadSpotPurchaseSignal.Dispatch(SpotPurchaseView.PowerUpSections.COACH);
                }
            };

            promotionCycle.Add(strengthItem);
            promotionCycle.Add(coachItem);
            promotionCycle.Add(ultimateItem);
            promotionCycle.Add(adsItem);
            promotionCycle.Add(strengthPurchase);
            promotionCycle.Add(coachPurchase);

            promotionCycle.Sort((x,y) => x.cycleIndex.CompareTo(y.cycleIndex));
        }
    }
}
