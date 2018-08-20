using System;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public interface IPicsModel
    {
        Sprite GetPic(string playerId);
        void SetPic(string playerId, Sprite sprite);
    }
}

