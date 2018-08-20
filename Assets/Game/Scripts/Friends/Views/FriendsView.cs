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
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantGame
{
    public class FriendsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

		public Transform listContainer;
		public Transform friendsSibling;
		public Transform communitySibling;
		public GameObject friendBarPrefab;
		public Text noFriendsBtnText;
		public Button noFriendsBtn;
		public Text friendsTitle;
		public Text inviteText;
		public Text communityTitle;
		public Text refreshText;
		public FriendDialog friendDlg;
		public GameObject confirmDlg;

        private Dictionary<string, GameObject> bars = new Dictionary<string, GameObject>();

        public void Init()
        {
			noFriendsBtnText.text = localizationService.Get(LocalizationKey.FRIENDS_NO_FRIENDS_TEXT);
			friendsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_TITLE);
			inviteText.text = localizationService.Get(LocalizationKey.FRIENDS_INVITE_TEXT);
			communityTitle.text = localizationService.Get(LocalizationKey.FRIENDS_COMMUNITY_TITLE);
			refreshText.text = localizationService.Get(LocalizationKey.FRIENDS_REFRESH_TEXT);
			friendDlg.confirmLabel.text = localizationService.Get(LocalizationKey.FRIENDS_CONFIRM_LABEL);
			friendDlg.yesLabel.text = localizationService.Get(LocalizationKey.FRIENDS_YES_LABEL);
			friendDlg.noLabel.text = localizationService.Get(LocalizationKey.FRIENDS_NO_LABEL);
			friendDlg.vsLabel.text = localizationService.Get(LocalizationKey.FRIENDS_VS_LABEL);
			friendDlg.winsLabel.text = localizationService.Get(LocalizationKey.FRIENDS_WINS_LABEL);
			friendDlg.drawsLabel.text = localizationService.Get(LocalizationKey.FRIENDS_DRAWS_LABEL);
			friendDlg.totalGamesLabel.text = localizationService.Get(LocalizationKey.FRIENDS_TOTAL_GAMES_LABEL);
			friendDlg.blockLabel.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCK_LABEL);
        }

		public void AddFriend(Friend friend)
		{
            if (bars.ContainsKey(friend.playerId))
                return;

		    // create bar
			GameObject friendBar = GameObject.Instantiate(friendBarPrefab);

            // update bar values
            FriendBar barData = friendBar.GetComponent<FriendBar>();
            barData.profileNameLabel.text = friend.publicProfile.name;
			friendBar.transform.SetParent(listContainer, false);
			friendBar.transform.SetSiblingIndex(friendsSibling.GetSiblingIndex() + 1);

            // store bar
            bars.Add(friend.playerId, friendBar);

            // hide default message
            noFriendsBtn.gameObject.SetActive(false);
		}

        public void UpdateFriendPic(string playerId, Sprite sprite)
        {
            FriendBar barData = bars[playerId].GetComponent<FriendBar>();
            barData.avatarImage.sprite = sprite;
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
