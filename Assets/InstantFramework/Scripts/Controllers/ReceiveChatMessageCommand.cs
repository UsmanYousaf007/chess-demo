/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.Multiplayer;
using System;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework 
{
    public class ReceiveChatMessageCommand : Command
    {
        // Parameters
        [Inject] public ChatMessage chatMessage { get; set; }
        [Inject] public bool isBackupMessage { get; set; }

        // Dispatch signals
        [Inject] public DisplayChatMessageSignal displayChatMessageSignal { get; set; }
        [Inject] public AddUnreadMessagesToBarSignal addUnreadMessagesSignal { get; set; }

        // Models
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

        public override void Execute()
        {
            if (!chatModel.AddChat(chatMessage.senderId, chatMessage, isBackupMessage))
            {
                return;
            }

            // Send message to view
            displayChatMessageSignal.Dispatch(chatMessage);

            // Setup the unread indicator
            if (navigatorModel.currentViewId != NavigatorViewId.MULTIPLAYER_CHAT_DLG)
            {
            	TLUtils.LogUtil.LogNullValidation(chatMessage.senderId, "chatMessage.senderId");
            	
                if (chatMessage.senderId != null)
                {
                    if (chatModel.hasUnreadMessages.ContainsKey(chatMessage.senderId))
                    {
                        chatModel.hasUnreadMessages[chatMessage.senderId] = true;
                    }
                    else
                    {
                        chatModel.hasUnreadMessages.Add(chatMessage.senderId, true);
                    }

                    addUnreadMessagesSignal.Dispatch(chatMessage.senderId);
                }
            }
        }
    }
}
