﻿/// @license Propriety <http://license.url>
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
    public class LoadFriendsCommand : Command
    {
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public FriendsConnectFacebookSignal friendsConnectFacebookSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_FRIENDS);

            if (facebookService.isLoggedIn())
            {
                friendsConnectFacebookSignal.Dispatch(false);
                refreshCommunitySignal.Dispatch();

            }
            else
            {
                friendsConnectFacebookSignal.Dispatch(true);
            }
        }
    }
}
