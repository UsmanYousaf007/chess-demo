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
        [Inject] public string chatMessage { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private string recipientId;

        public override void Execute()
        {
            Retain();

            recipientId = matchInfoModel.activeLongMatchOpponentId;
            backendService.SendChatMessage(recipientId, chatMessage).Then(OnMessageSent);
        }

        private void OnMessageSent(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                ChatMessage message;
                message.senderId = playerModel.id;
                message.recipientId = recipientId;
                message.text = chatMessage;
                message.timestamp = TimeUtil.unixTimestampMilliseconds;

                chatModel.AddChat(recipientId, message);
            }

            Release();
        }
    }
}
