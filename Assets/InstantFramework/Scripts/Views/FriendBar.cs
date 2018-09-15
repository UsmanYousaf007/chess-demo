using UnityEngine;
using UnityEngine.UI;
using System;

namespace TurboLabz.InstantFramework
{
    public class FriendBar : MonoBehaviour
    {
        public Image avatarImage;
        public Text profileNameLabel;
        public Text eloScoreLabel;
        public GameObject timer;
        public Text timerLabel;
        public Text statusLabel;
        public Button viewProfileButton;
        public Button playButton;
        public Friend friendInfo;
        public DateTime lastActionTime;
        public LongPlayStatus longPlayStatus;
        public GameObject thinking;
        public SkinLink incDecSkinLink;
        public bool isCommunity;

    }
}