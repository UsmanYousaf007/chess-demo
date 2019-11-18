/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class LoadChatCommand : Command
    {
        //Parameters
        [Inject] public string opponentId { get; set; }

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

            PublicProfile opponentProfile = null;

            if (playerModel.friends.ContainsKey(opponentId))
            {
                opponentProfile = playerModel.friends[opponentId].publicProfile;
            }
            else if (playerModel.community.ContainsKey(opponentId))
            {
                opponentProfile = playerModel.community[opponentId].publicProfile;
            }
            else if (playerModel.search.ContainsKey(opponentId))
            {
                opponentProfile = playerModel.search[opponentId].publicProfile;
            }

            //if opponentProfile is still null then it means match is made using random quick match
            //getting opponent profile from active match
            if (opponentProfile == null)
            {
                opponentProfile = matchInfoModel.activeMatch.opponentPublicProfile;
            }

            ChatVO vo = new ChatVO();
            vo.chatMessages = chatModel.GetChat(opponentId);
            vo.playerId = playerModel.id;
            vo.playerProfilePic = picsModel.GetPlayerPic(playerModel.id);
            vo.avatarBgColorId = playerModel.avatarBgColorId;
            vo.avatarId = playerModel.avatarId;
            vo.opponentProfilePic = picsModel.GetPlayerPic(opponentId);

            if (opponentProfile != null)
            {
                vo.opponentId = opponentId;
                vo.opponentName = opponentProfile.name;
                vo.oppAvatarId = opponentProfile.avatarId;
                vo.oppAvatarBgColorId = opponentProfile.avatarBgColorId;
                vo.isOnline = opponentProfile.isOnline;
                vo.isActive = opponentProfile.isActive;

                if (vo.opponentProfilePic == null)
                {
                    vo.opponentProfilePic = opponentProfile.profilePicture;
                }
            }

            vo.hasUnreadMessages = chatModel.hasUnreadMessages.ContainsKey(opponentId);
            vo.unreadMessagesCount = 0;

            if (vo.hasUnreadMessages)
            {
                vo.unreadMessagesCount = chatModel.hasUnreadMessages[opponentId];
            }

            updateChatView.Dispatch(vo);
        }
    }
}