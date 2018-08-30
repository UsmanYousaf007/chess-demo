/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-11 13:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using TurboLabz.Multiplayer;


namespace TurboLabz.InstantFramework
{
    public class LongMatchReadyCommand : Command
    {
        // Parameters
        [Inject] public MatchIdVO matchId { get; set; }

        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public UpdateOpponentProfileSignal updateOpponentProfileSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }


        // TODO: Seperate game specific material to the game folder
        public override void Execute()
        {
            Retain();

            // Is the opponent on the block list? If so, exit
            if (playerModel.blocked.ContainsKey(matchId.opponentId))
            {
                return;
            }

            // A friend or visible community member starts a new game with you
            if (playerModel.friends.ContainsKey(matchId.opponentId) ||
                playerModel.community.ContainsKey(matchId.opponentId))
            {
                
            }

            MatchInfo matchInfo = matchInfoModel.matches[matchId.challengeId];
            DateTime startTime = TimeUtil.ToDateTime(matchInfo.gameStartTimeMilliseconds);

            LongPlayStatusVO vo = new LongPlayStatusVO();
            vo.lastActionTime = startTime;

            // If you didn't start this match then this person has challenged you
            if (matchId.opponentId != matchInfoModel.activeLongMatchOpponentId)
            {
                vo.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
            }
            // else set it to the person who's turn it is
            else
            {
                Chessboard chessboard = chessboardModel.chessboards[matchId.challengeId];
                vo.longPlayStatus = (chessboard.currentTurnPlayerId == matchId.opponentId) ?
                    LongPlayStatus.OPPONENT_TURN : LongPlayStatus.PLAYER_TURN;
            }

            vo.playerId = matchId.opponentId;
            updateFriendBarSignal.Dispatch(vo);

            // Launch the game if you tapped a player
            if (matchId.opponentId == matchInfoModel.activeLongMatchOpponentId)
            {
                matchInfoModel.activeChallengeId = matchId.challengeId;
                PublicProfile publicProfile = matchInfo.opponentPublicProfile;

                ProfileVO pvo = new ProfileVO();
                pvo.playerName = publicProfile.name;
                pvo.eloScore = publicProfile.eloScore;
                pvo.countryId = publicProfile.countryId;

                if (playerModel.friends.ContainsKey(matchId.opponentId))
                {
                    pvo.playerPic = playerModel.friends[matchId.opponentId].publicProfile.profilePicture;
                }
                else if (playerModel.community.ContainsKey(matchId.opponentId))
                {
                    pvo.playerPic = playerModel.community[matchId.opponentId].publicProfile.profilePicture;
                }

                updateOpponentProfileSignal.Dispatch(pvo);

                startGameSignal.Dispatch();
            }
        }

        /*

        private void OnFindMatch(BackendResult result)
        {
            if (result == BackendResult.CANCELED)
            {
                Release();
            }
            else if (result != BackendResult.SUCCESS)
            {
              //  backendErrorSignal.Dispatch(result);
                Release();
            }
        }

        private void OnFindMatchComplete(string challengeId)
        {
            matchInfoModel.activeChallengeId = challengeId;
            PublicProfile opponentPublicProfile = matchInfoModel.activeMatch.opponentPublicProfile;

            if (opponentPublicProfile.facebookUserId != null)
            {
                facebookService.GetSocialPic(opponentPublicProfile.facebookUserId, opponentPublicProfile.playerId).Then(OnGetOpponentProfilePicture);
            }
            else
            {
                MatchFound();
            }
        }

        private void OnGetOpponentProfilePicture(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                matchInfoModel.activeMatch.opponentPublicProfile.profilePicture = sprite;
            }
            else
            {
                // In case of a failure we just don't set the profile picture.
                LogUtil.LogWarning("Unable to get the profile picture. FacebookResult: " + result);
            }

            MatchFound();
        }

        private void MatchFound()
        {
            // Create and dispatch opponent profile with the match found signal
            PublicProfile publicProfile = matchInfoModel.activeMatch.opponentPublicProfile;

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = publicProfile.profilePicture;
            pvo.playerName = publicProfile.name;
            pvo.eloScore = publicProfile.eloScore;
            pvo.countryId = publicProfile.countryId;

            //matchFoundSignal.Dispatch(pvo);
            //getGameStartTimeSignal.Dispatch();

            Release();
        }
        */
    }
}
