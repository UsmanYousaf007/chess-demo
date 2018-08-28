﻿/// @license Propriety <http://license.url>
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
using System;

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
        public Button inviteFriendsButton;
        public Text inviteText;
		public Text communityTitle;
        public Button refreshCommunityButton;
		public Text refreshText;
		public GameObject confirmDlg;
        public Button facebookLoginButton;
        public Text facebookLoginButtonText;
        public GameObject facebookConnectAnim;
        public Text facebookConnectText;
        public ScrollRect scrollRect;

        public Signal facebookButtonClickedSignal = new Signal();
        public Signal reloadFriendsSignal = new Signal();
        public Signal refreshCommunitySignal = new Signal();
        public Signal<string> showProfileDialogSignal = new Signal<string>();
        public Signal<string> playButtonClickedSignal = new Signal<string>();

        private Dictionary<string, GameObject> bars = new Dictionary<string, GameObject>();
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


        public void AddFriends(Dictionary<string, Friend> friends)
        {
            foreach (KeyValuePair<string, Friend> entry in friends)
            {
                AddFriend(entry.Value);
            }

            UpdateAllStatus();
        }

		void AddFriend(Friend friend)
		{
		    // create bar
			GameObject friendBarObj = GameObject.Instantiate(friendBarPrefab);

            // update bar values
            FriendBar friendBar = friendBarObj.GetComponent<FriendBar>();
            friendBar.viewProfileButton.onClick.AddListener(() => ViewProfile(friend.playerId));
            friendBar.playButton.onClick.AddListener(() => PlayButtonClicked(friend.playerId));
            friendBar.friendInfo = friend;
            friendBar.profileNameLabel.text = friend.publicProfile.name;
            friendBar.eloScoreLabel.text = friend.publicProfile.eloScore.ToString();
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

            bars.Add(friend.playerId, friendBarObj);

            UpdateFriendPic(friend.playerId, friend.publicProfile.profilePicture);
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

        public void UpdateFriendBar(LongPlayStatusVO vo)
        {
            if (!bars.ContainsKey(vo.playerId))
                return;

            FriendBar friendBar = bars[vo.playerId].GetComponent<FriendBar>();
            friendBar.lastActionTime = vo.lastActionTime;
            friendBar.longPlayStatus = vo.longPlayStatus;
            UpdateStatus(friendBar);
        }

        public void Show() 
        { 
            gameObject.SetActive(true); 
            UpdateAllStatus();
        }

        public void Hide()
        { 
            gameObject.SetActive(false); 
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void ClearCommunity()
        {
            ClearType(Friend.FRIEND_TYPE_COMMUNITY);
            waitingForPlayersText.gameObject.SetActive(true);
        }

        public void ClearFriends()
        {
            ClearType(Friend.FRIEND_TYPE_SOCIAL);
        }

        void ClearType(string friendType)
        {
            List<string> destroyMe = new List<string>();

            foreach (KeyValuePair<string, GameObject> entry in bars)
            {
                if (entry.Value.GetComponent<FriendBar>().friendInfo.type == friendType)
                {
                    destroyMe.Add(entry.Key);
                }    
            }

            foreach (string key in destroyMe)
            {
                GameObject.Destroy(bars[key]);
                bars.Remove(key);
            }
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

        void PlayButtonClicked(string playerId)
        {
            playButtonClickedSignal.Dispatch(playerId);
        }

        void UpdateAllStatus()
        {
            foreach (KeyValuePair<string, GameObject> entry in bars)
            {
                FriendBar friendBar = entry.Value.GetComponent<FriendBar>(); 
                UpdateStatus(friendBar);
            }
        }

        void UpdateStatus(FriendBar friendBar)
        {
            friendBar.statusLabel.gameObject.SetActive(true);

            // Update status
            if (friendBar.longPlayStatus == LongPlayStatus.NONE)
            {
                friendBar.statusLabel.gameObject.SetActive(false);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.NEW_CHALLENGE)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_CHALLENGED_YOU);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.PLAYER_TURN)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.OPPONENT_TURN)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_THEIR_TURN);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.PLAYER_WON)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_WON);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.OPPONENT_WON)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_LOST);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.DRAW)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DRAW);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.DECLINED)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DECLINED);
            }

            // Update timers
            if (friendBar.longPlayStatus != LongPlayStatus.NEW_CHALLENGE &&
                friendBar.longPlayStatus != LongPlayStatus.PLAYER_TURN &&
                friendBar.longPlayStatus != LongPlayStatus.OPPONENT_TURN)
            {
                friendBar.timer.gameObject.SetActive(false);
                friendBar.timerLabel.gameObject.SetActive(false);
                return;
            }

            friendBar.timer.gameObject.SetActive(true);
            friendBar.timerLabel.gameObject.SetActive(true);

            TimeSpan elapsedTime = DateTime.UtcNow.Subtract(friendBar.lastActionTime);

            if (elapsedTime.TotalHours < 1)
            {
                friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_MINUTES, 
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalMinutes)));
            }
            else if (elapsedTime.TotalDays < 1)
            {
                friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_HOURS, 
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalHours)));
            }
            else
            {
                friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DAYS, 
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalDays)));
            }
        }
            
    }
}
