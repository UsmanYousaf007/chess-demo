/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class FriendsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

		public Transform friendsSibling;
		public Transform communitySibling;
		public GameObject friendBarPrefab;
		public Text noFriendsBtnText;
		public Button noFriendsBtn;
		public Text friendsTitle;
		public Text inviteText;
		public Text communityTitle;
		public Text refreshText;



        public void Init()
        {
        //    comingSoonLabel.text = localizationService.Get(LocalizationKey.FRIENDS_COMING_SOON);
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }
    }
}
