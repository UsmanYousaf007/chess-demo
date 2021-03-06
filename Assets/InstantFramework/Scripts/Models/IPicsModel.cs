using System;
using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IPicsModel
    {
        Sprite GetPlayerPic(string playerId);

        void SetPlayerPic(string playerId, Sprite sprite, bool saveOnDisk = true);

        Dictionary<string, Sprite> GetFriendPics(List<string> playerIds);

        void SetFriendPics(Dictionary<string, Friend> pics, bool saveOnDisk = true);

        void DeleteFriendPic(string playerId);
    }
}

