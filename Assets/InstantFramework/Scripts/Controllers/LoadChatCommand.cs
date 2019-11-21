/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class LoadChatCommand : Command
    {
        //Parameters
        [Inject] public string opponentId { get; set; }
        [Inject] public bool inGameChat { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateChatView updateChatView { get; set; }

        //Models
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            TLUtils.LogUtil.LogNullValidation(opponentId, "opponentId");

            if (opponentId == null) return;

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHAT);
            chatModel.activeChatId = opponentId;

            Friend opponentProfile = playerModel.GetFriend(opponentId);
            PublicProfile opponentPublicProfile = null;

            //if opponentProfile is still null then it means match is made using random quick match
            //getting opponent profile from active match
            if (opponentProfile == null)
            {
                if (matchInfoModel.activeMatch != null)
                {
                    opponentPublicProfile = matchInfoModel.activeMatch.opponentPublicProfile;
                }
                else
                {
                    opponentPublicProfile = matchInfoModel.lastCompletedMatch.opponentPublicProfile;
                }

                opponentPublicProfile.isOnline = true;
            }
            else
            {
                opponentPublicProfile = opponentProfile.publicProfile;
            }

            ChatVO vo = new ChatVO();
            vo.chatMessages = chatModel.GetChat(opponentId);
            vo.playerId = playerModel.id;
            vo.playerProfilePic = picsModel.GetPlayerPic(playerModel.id);
            vo.avatarBgColorId = playerModel.avatarBgColorId;
            vo.avatarId = playerModel.avatarId;
            vo.inGame = inGameChat;
            vo.opponentProfilePic = picsModel.GetPlayerPic(opponentId);

            if (opponentPublicProfile != null)
            {
                vo.opponentId = opponentId;
                vo.opponentName = opponentPublicProfile.name;
                vo.oppAvatarId = opponentPublicProfile.avatarId;
                vo.oppAvatarBgColorId = opponentPublicProfile.avatarBgColorId;
                vo.isOnline = opponentPublicProfile.isOnline;
                vo.isActive = opponentPublicProfile.isActive;

                if (vo.opponentProfilePic == null)
                {
                    vo.opponentProfilePic = opponentPublicProfile.profilePicture;
                }
            }

            vo.hasUnreadMessages = chatModel.hasUnreadMessages.ContainsKey(opponentId);
            vo.unreadMessagesCount = 0;

            if (vo.hasUnreadMessages)
            {
                vo.unreadMessagesCount = chatModel.hasUnreadMessages[opponentId];
            }

            vo.isChatEnabled = true;

            //chat will be disbaled under these conditions
            //1. no match completed with player
            //2. long match with current player is not running with player
            //3. no quick match is progress
            //in other words chat will only be enabled in case if match is started with the player
            if (opponentProfile != null && opponentProfile.lastMatchTimestamp <= 0)
            {
                var activeMatch = GetActiveMatchWithOpponent();
                if (activeMatch != null)
                {
                    if (activeMatch.isLongPlay)
                    {
                        if (activeMatch.acceptStatus != GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED)
                        {
                            vo.isChatEnabled = false;
                        }
                    }
                    else
                    {
                        if (!activeMatch.opponentPublicProfile.playerId.Equals(opponentId))
                        {
                            vo.isChatEnabled = false;
                        }
                    }
                }
                else
                {
                    vo.isChatEnabled = false;
                }
            }

            updateChatView.Dispatch(vo);
        }

        private MatchInfo GetActiveMatchWithOpponent()
        {
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId.Equals(opponentId))
                {
                    return entry.Value;
                }
            }
            return null;
        }

    }
}
