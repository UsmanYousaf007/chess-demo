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
    public class ClearUnreadMessagesCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch Signals
        [Inject] public ClearUnreadMessagesFromBarSignal clearUnreadMessagesFromBarSignal { get; set; }

        // Models
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            if (chatModel.hasUnreadMessages.ContainsKey(opponentId))
            {
                chatModel.hasUnreadMessages.Remove(opponentId);
                clearUnreadMessagesFromBarSignal.Dispatch(opponentId);
            }
        }
    }
}
