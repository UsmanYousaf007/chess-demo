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
    public class RefreshFriendsCommand : Command
    {
        // dispatch signals
        [Inject] public ClearFriendsSignal clearFriendsSignal { get; set; }
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }
        [Inject] public GetSocialPicsSignal getSocialPicsSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }

        // models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public override void Execute()
        {
            clearFriendsSignal.Dispatch();

            List<string> keyList = new List<string>(playerModel.friends.Keys);
            Dictionary<string, Sprite> pics = picsModel.GetFriendPics(keyList);

            if (pics != null)
            {
                foreach (KeyValuePair<string, Sprite> pic in pics)
                {
                    playerModel.friends[pic.Key].publicProfile.profilePicture = pic.Value;
                }
            }

            addFriendsSignal.Dispatch(playerModel.friends, false);
            getSocialPicsSignal.Dispatch(playerModel.friends);

            foreach (string key in playerModel.friends.Keys)
            {
                updateFriendBarSignal.Dispatch(key);
            }

            sortFriendsSignal.Dispatch();
        }
    }
}
