using System;
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.Implementation.Model
{
    [Serializable]
    public class TileModel
    {
        [SerializeField] string uuid = default;
        [SerializeField] string title = default;
        [SerializeField] string spritePath = default;
        [SerializeField] string androidStoreLink = default;
        [SerializeField] string iOSStoreLink = default;
        [SerializeField] string androidPackageName = default;
        [SerializeField] string iOSPackageName = default;
        [SerializeField] string iOSURLScheme = default;
        [SerializeField] bool isRemote = default;
        [SerializeField] int priority = default;
        [SerializeField] bool isInteractive = true;
        [SerializeField] bool isButtonActive = true;

        public string Uuid => uuid;
        public string Title => title;
        public string SpritePath => spritePath;

        public string StoreLink => 
            Application.platform == RuntimePlatform.IPhonePlayer 
                ? iOSStoreLink 
                : androidStoreLink;

        public string PackageName =>
            Application.platform == RuntimePlatform.IPhonePlayer
                ? iOSPackageName
                : androidPackageName;

        public string URLScheme => iOSURLScheme;

        public bool IsRemote => isRemote;
        public int Priority => priority;
        public bool IsInteractive => isInteractive;
        public bool IsButtonActive => isButtonActive;
    }
}