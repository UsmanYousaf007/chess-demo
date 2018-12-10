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
    public class SendChatMessageCommand : Command
    {
        // Parameters
        [Inject] public ChatMessage chatMessage { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            chatModel.AddChat(chatMessage.recipientId, chatMessage, false);
            backendService.SendChatMessage(chatMessage.recipientId, chatMessage.text, chatMessage.guid);

            if (!chatModel.hasEngagedChat) 
            {
                chatModel.hasEngagedChat = true;
                analyticsService.ChatEngaged();
            }
        }
    }
}
