using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class LoadCareerCardCommand : Command
    {
        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Dispatch Signals
        [Inject] public UpdateCareerCardSignal updateCareerCardSignal { get; set; }

        private static long playerCoins;

        public override void Execute()
        {
            var coinsStockChanged = playerCoins != playerModel.coins;
            playerCoins = playerModel.coins;

            var gamesPlayedIndex = preferencesModel.gamesPlayedPerDay;
            var lastIndex = metaDataModel.settingsModel.defaultBetIncrementByGamesPlayed.Count - 1;
            gamesPlayedIndex = gamesPlayedIndex >= lastIndex ? lastIndex : gamesPlayedIndex;

            var coinsToBet = playerModel.coins * metaDataModel.settingsModel.defaultBetIncrementByGamesPlayed[gamesPlayedIndex];
            var minCoinsToBet = playerModel.coins * metaDataModel.settingsModel.defaultBetIncrementByGamesPlayed[0];

            var vo = new CareerCardVO();
            vo.betIndex = GetBetIndex(coinsToBet); ;
            vo.minimumBetIndex = GetBetIndex(minCoinsToBet);
            vo.coinsStockChanged = coinsStockChanged;
            updateCareerCardSignal.Dispatch(vo);
        }

        private int GetBetIndex(float coinsToBet)
        {
            var betIndex = 0;

            for (int i = 0; i < metaDataModel.settingsModel.bettingIncrements.Count; i++)
            {
                if (coinsToBet <= metaDataModel.settingsModel.bettingIncrements[i])
                {
                    if (i == 0)
                    {
                        betIndex = i;
                        break;
                    }

                    var diff1 = Mathf.Abs(coinsToBet - metaDataModel.settingsModel.bettingIncrements[i - 1]);
                    var diff2 = Mathf.Abs(coinsToBet - metaDataModel.settingsModel.bettingIncrements[i]);
                    betIndex = diff1 < diff2 ? i - 1 : i;
                    break;
                }

                betIndex = i;
            }

            return betIndex;
        }
    }
}
