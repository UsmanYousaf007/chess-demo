/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class RemoveFriendCommand : Command
    {
        // parameters
        [Inject] public string friendId { get; set; }

        // dispatch signals
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            TLUtils.LogUtil.LogNullValidation(friendId, "friendId");

            if (friendId == null) return;

            if (playerModel.friends.ContainsKey(friendId))
            {
                playerModel.friends.Remove(friendId);
            }
            else if (playerModel.community.ContainsKey(friendId))
            {
                playerModel.community.Remove(friendId);
            }

            clearFriendSignal.Dispatch(friendId);
        }
    }
}
