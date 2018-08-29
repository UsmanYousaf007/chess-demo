using System;
using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IPicsModel
    {
        Sprite GetPlayerPic(string playerId);
        void SetPlayerPic(string playerId, Sprite sprite);

        Dictionary<string, Sprite> GetFriendPics(List<string> playerIds);
        void SetFriendPics(Dictionary<string, Friend> pics);
    }
}

