/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer 
{
    public class ResetActiveMatchCommand : Command
    {
        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            if (matchInfoModel.activeChallengeId != null)
            {
                if (chessboardModel.chessboards.ContainsKey(matchInfoModel.activeChallengeId))
                {
                    chessboardModel.chessboards[matchInfoModel.activeChallengeId].currentState = null;
                }

                matchInfoModel.activeChallengeId = null;
                matchInfoModel.activeLongMatchOpponentId = null;
            }
        }
    }
}
