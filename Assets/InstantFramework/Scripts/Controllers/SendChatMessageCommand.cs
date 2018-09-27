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
        [Inject] public ChatMessageVO chatMessage { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IChatModel chatModel { get; set; }

        private string opponentId;

        public override void Execute()
        {
            Retain();
            backendService.SendChatMessage(chatMessage.recipientId, chatMessage.text);
        }

        private void OnUnregister(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                Dictionary<string, ChatMessages> chatHistory = chatModel.chatHistory;

                ChatMessage message;
                message.recipientId = chatMessage.recipientId;
                message.text = chatMessage.text;
                message.timestamp = TimeUtil.unixTimestampMilliseconds;

                if (!chatHistory.ContainsKey(chatMessage.recipientId))
                {
                    chatHistory.Add(message.recipientId, new ChatMessages());
                }

                chatHistory[chatMessage.recipientId].messageList.Add(message);
            }

            Release();
        }
    }
}
