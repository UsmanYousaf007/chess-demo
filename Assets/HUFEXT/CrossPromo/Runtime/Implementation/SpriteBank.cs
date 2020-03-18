using System.Collections.Generic;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation
{
    public static class SpriteBank
    {
        public static Dictionary<string, Sprite> Sprites;

        static SpriteBank()
        {
            Sprites = new Dictionary<string, Sprite>();
        }
    }
}