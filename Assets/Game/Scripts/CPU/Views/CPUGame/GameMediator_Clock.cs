/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:02:14 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using TurboLabz.Chess;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess 
{
    public partial class GameMediator
    {
        public void OnRegisterClock()
        {
            view.InitClock();
        }

        public void OnRemoveClock()
        {
            view.CleanupClock();
        }

        [ListensTo(typeof(InitTimersSignal))]
        public void InitTimersSignal(InitTimerVO vo)
        {
            view.InitTimers(vo);
        }

        [ListensTo(typeof(InitInfiniteTimersSignal))]
        public void InitInfiniteTimersSignal(bool isPlayerTurn)
        {
            view.InitInfiniteTimers(isPlayerTurn);
        }

        [ListensTo(typeof(TakeTurnSwapTimeControlSignal))]
        public void OnTakeTurnSwapTimeControl()
        {
            LogUtil.Log("Swap time control....", "red");
            view.PlayerTurnComplete();
        }

        [ListensTo(typeof(ReceiveTurnSwapTimeControlSignal))]
        public void OnReceiveTurnSwapTimeControl()
        {
            view.OpponentTurnComplete();
        }

        [ListensTo(typeof(UpdatePlayerTimerSignal))]
        public void OnUpdatePlayerTimer(TimeSpan playerTimer)
        {
            view.TickPlayerTimer(playerTimer);
        }

        [ListensTo(typeof(UpdateOpponentTimerSignal))]
        public void OnUpdateOpponentTimer(TimeSpan opponentTimer)
        {
            view.TickOpponentTimer(opponentTimer);
        }

        [ListensTo(typeof(PlayerTimerExpiredSignal))]
        public void OnPlayerTimerExpired()
        {
            view.ExpirePlayerTimer();
        }

        [ListensTo(typeof(OpponentTimerExpiredSignal))]
        public void OnOpponentTimerExpired()
        {
            view.ExpireOpponentTimer();
        }
    }
}
