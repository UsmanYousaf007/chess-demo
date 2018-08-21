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
using strange.extensions.signal.impl;

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
        public Text waitingForPlayersText;
		public Button noFriendsBtn;
		public Text friendsTitle;
		public Text inviteText;
		public Text communityTitle;
		public Text refreshText;
		public FriendDialog friendDlg;
		public GameObject confirmDlg;
        public Button facebookLoginButton;
        public Text facebookLoginButtonText;
        public GameObject facebookConnectAnim;
        public Text facebookConnectText;
        public ScrollRect scrollRect;

        public Signal facebookButtonClickedSignal = new Signal();
        public Signal reloadFriendsSignal = new Signal();

        private Dictionary<string, GameObject> bars = new Dictionary<string, GameObject>();
        private List<FriendBar> communityBars = new List<FriendBar>();

        public void Init()
        {
            facebookLoginButtonText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_LOGIN_BUTTON_TEXT);
            facebookConnectText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT);
            noFriendsBtnText.text = localizationService.Get(LocalizationKey.FRIENDS_NO_FRIENDS_TEXT);
            waitingForPlayersText.text = localizationService.Get(LocalizationKey.FRIENDS_WAITING_FOR_PLAYERS);
            facebookConnectText.text = localizationService.Get(LocalizationKey.FRIENDS_FACEBOOK_CONNECT_TEXT);
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

            facebookLoginButton.onClick.AddListener(OnFacebookButtonClicked);
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
			GameObject friendBar = GameObject.Instantiate(friendBarPrefab);

            // update bar values
            FriendBar barData = friendBar.GetComponent<FriendBar>();
            barData.playerId = friend.playerId;
            barData.profileNameLabel.text = friend.publicProfile.name;
			friendBar.transform.SetParent(listContainer, false);

            int siblingIndex = 0;

            if (friend.type == Friend.FRIEND_TYPE_SOCIAL)
            {
                siblingIndex = friendsSibling.GetSiblingIndex() + 1;
                noFriendsBtn.gameObject.SetActive(false);
            }
            else 
            {
                siblingIndex = communitySibling.GetSiblingIndex() + 1;
                waitingForPlayersText.gameObject.SetActive(false);
            }
            
            friendBar.transform.SetSiblingIndex(siblingIndex);

            // store bar
            bars.Add(friend.playerId, friendBar);

            // save community bars for refresh
            if (friend.type == Friend.FRIEND_TYPE_COMMUNITY)
            {
                communityBars.Add(barData);
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
                bars.Remove(barData.playerId);
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
    }
}
