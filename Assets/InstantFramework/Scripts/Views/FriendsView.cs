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
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class FriendsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

		public Transform listContainer;
		public Transform friendsSibling;
		public Transform communitySibling;
		public GameObject friendBarPrefab;
        public Text[] defaultInviteFriendsNewLines;
        public Text defaultInviteFriendsText;
        public Text defaultInviteFriendsButtonText;
        public Button defaultInviteFriendsButton;
        public Text waitingForPlayersText;
		public Text friendsTitle;
		public Text inviteText;
		public Text communityTitle;
		public Text refreshText;
		public GameObject confirmDlg;
        public Button facebookLoginButton;
        public Text facebookLoginButtonText;
        public GameObject facebookConnectAnim;
        public Text facebookConnectText;
        public ScrollRect scrollRect;

        public Signal facebookButtonClickedSignal = new Signal();
        public Signal reloadFriendsSignal = new Signal();
        public Signal<string> showProfileDialogSignal = new Signal<string>();

        private Dictionary<string, GameObject> bars = new Dictionary<string, GameObject>();
        private List<FriendBar> communityBars = new List<FriendBar>();
        private List<GameObject> defaultInvite = new List<GameObject>();

        public void Init()
        {
            facebookLoginButtonText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT);
            facebookConnectText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT);
            defaultInviteFriendsButtonText.text = localizationService.Get(LocalizationKey.FRIENDS_INVITE_TEXT);
            defaultInviteFriendsText.text = localizationService.Get(LocalizationKey.FRIENDS_NO_FRIENDS_TEXT);
            waitingForPlayersText.text = localizationService.Get(LocalizationKey.FRIENDS_WAITING_FOR_PLAYERS);
            facebookConnectText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT);
			friendsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_TITLE);
			inviteText.text = localizationService.Get(LocalizationKey.FRIENDS_INVITE_TEXT);
			communityTitle.text = localizationService.Get(LocalizationKey.FRIENDS_COMMUNITY_TITLE);
			refreshText.text = localizationService.Get(LocalizationKey.FRIENDS_REFRESH_TEXT);

            facebookLoginButton.onClick.AddListener(OnFacebookButtonClicked);

            defaultInvite.Add(defaultInviteFriendsText.gameObject);
            defaultInvite.Add(defaultInviteFriendsButtonText.gameObject);
            defaultInvite.Add(defaultInviteFriendsButton.gameObject);
            for (int i = 0; i < defaultInviteFriendsNewLines.Length; i++)
                defaultInvite.Add(defaultInviteFriendsNewLines[i].gameObject);
        }

        public void ShowConnectFacebook(bool showConnectInfo)
        {
            if (showConnectInfo)
            {
                listContainer.gameObject.SetActive(false);
                facebookLoginButton.gameObject.SetActive(true);
                facebookLoginButton.enabled = true;
                facebookConnectText.gameObject.SetActive(true);
                facebookConnectAnim.SetActive(false);
            }
            else
            {
                listContainer.gameObject.SetActive(true);
                facebookLoginButton.gameObject.SetActive(false);
                facebookLoginButton.enabled = false;
                facebookConnectText.gameObject.SetActive(false);
                facebookConnectAnim.SetActive(false); 
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        public void FacebookAuthResult(bool isSuccessful, Sprite pic, string name)
        {
            if (isSuccessful)
            {
                reloadFriendsSignal.Dispatch();
            }
            else
            {
                ShowConnectFacebook(true);
            }
        }

		public void AddFriend(Friend friend)
		{
            if (bars.ContainsKey(friend.playerId))
                return;

		    // create bar
			GameObject friendBarObj = GameObject.Instantiate(friendBarPrefab);

            // update bar values
            FriendBar friendBar = friendBarObj.GetComponent<FriendBar>();
            friendBar.viewProfileButton.onClick.AddListener(() => ViewProfile(friend.playerId));
            friendBar.friendInfo = friend;
            friendBar.profileNameLabel.text = friend.publicProfile.name;
			friendBarObj.transform.SetParent(listContainer, false);

            int siblingIndex = 0;

            if (friend.type == Friend.FRIEND_TYPE_SOCIAL)
            {
                siblingIndex = friendsSibling.GetSiblingIndex() + 1;
                DefaultInviteSetActive(false);
            }
            else 
            {
                siblingIndex = communitySibling.GetSiblingIndex() + 1;
                waitingForPlayersText.gameObject.SetActive(false);
            }
            
            friendBarObj.transform.SetSiblingIndex(siblingIndex);

            // store bar
            bars.Add(friend.playerId, friendBarObj);

            // save community bars for refresh
            if (friend.type == Friend.FRIEND_TYPE_COMMUNITY)
            {
                communityBars.Add(friendBar);
            }
		}

        public void UpdateFriendPic(string playerId, Sprite sprite)
        {
            if (sprite == null)
                return;

            if (!bars.ContainsKey(playerId))
                return;

            FriendBar barData = bars[playerId].GetComponent<FriendBar>();
            barData.avatarImage.sprite = sprite;
        }

        public void ClearCommunity()
        {
            foreach (FriendBar barData in communityBars)
            {
                bars.Remove(barData.friendInfo.playerId);
                GameObject.Destroy(barData.gameObject);
            }

            communityBars.Clear();
            waitingForPlayersText.gameObject.SetActive(true);
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        void OnFacebookButtonClicked()
        {
            facebookButtonClickedSignal.Dispatch();
            facebookConnectAnim.SetActive(true);
            facebookLoginButton.enabled = false;
        }

        void DefaultInviteSetActive(bool active)
        {
            foreach (GameObject obj in defaultInvite)
            {
                obj.SetActive(active);
            }
        }

        void ViewProfile(string playerId)
        {
            showProfileDialogSignal.Dispatch(playerId);
        }
    }
}
