using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;

namespace TurboLabz.InstantGame
{
    public class LoadPromotionCommand : Command
    {
        //Singals
        [Inject] public ShowPromotionSignal showPromotionSignal { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private static string lastLoadedBanner = "none";
        private const int TOTAL_PROMOTIONS = 6;
        private const int POWERUP_USE_LIMIT = 7;
        private List<PromotionCycleItem> promotionCycle;

        private struct PromotionCycleItem
        {
            public int cycleIndex;
            public string key;
            public delegate bool Condition();
            public Condition condition;
        }

        public override void Execute()
        {
            if (!preferencesModel.isLobbyLoadedFirstTime)
            {
                preferencesModel.isLobbyLoadedFirstTime = true;
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
                DispatchSignal(promotionCycle[promotionToShowIndex].key);
                return;
            }

            if (!lastLoadedBanner.Equals("none"))
            {
                DispatchSignal("none");
                return;
            }
        }

        private void IncrementPromotionCycleIndex()
        {
            preferencesModel.promotionCycleIndex++;
            preferencesModel.promotionCycleIndex = preferencesModel.promotionCycleIndex > TOTAL_PROMOTIONS ? 1 : preferencesModel.promotionCycleIndex;
        }

        private void DispatchSignal(string key)
        {
            showPromotionSignal.Dispatch(key);
            lastLoadedBanner = key;
        }

        private void Init()
        {
            promotionCycle = new List<PromotionCycleItem>();

            var strengthItem = new PromotionCycleItem
            {
                cycleIndex = 1,
                key = LobbyPromotionKeys.STRENGTH_BANNER,
                condition = delegate
                {
                    return preferencesModel.strengthUsedCount < POWERUP_USE_LIMIT;
                }
            };

            var coachItem = new PromotionCycleItem
            {
                cycleIndex = 2,
                key = LobbyPromotionKeys.COACH_BANNER,
                condition = delegate
                {
                    return preferencesModel.coachUsedCount < POWERUP_USE_LIMIT;
                }
            };

            var ultimateItem = new PromotionCycleItem
            {
                cycleIndex = 3,
                key = LobbyPromotionKeys.ULTIMATE_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG);
                }
            };

            var adsItem = new PromotionCycleItem
            {
                cycleIndex = 4,
                key = LobbyPromotionKeys.ADS_BANNER,
                condition = delegate
                {
                    return !playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_NOADSFOREVER_SHOP_TAG);
                }
            };

            var strengthPurchase = new PromotionCycleItem
            {
                cycleIndex = 5,
                key = LobbyPromotionKeys.STRENGTH_PURCHASE,
                condition = delegate
                {
                    return true;
                }
            };

            var coachPurchase = new PromotionCycleItem
            {
                cycleIndex = 6,
                key = LobbyPromotionKeys.COACH_PURCHASE,
                condition = delegate
                {
                    return true;
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
