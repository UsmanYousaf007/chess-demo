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
using System;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public partial class FriendsView : View
    {
        [Header("Active and Recently Played")]
        public Image notificationTagImage;
        public Text notificationTagNumber;
        public Transform sectionActiveMatches;
        public Transform sectionRecentlyCompletedMatches;
        public Text sectionActiveMatchesTitle;
        public Text sectionRecentlyCompletedMatchesTitle;

        private List<FriendBar> recentlyCompleted = new List<FriendBar>();
        private const long RECENTLY_COMPLETED_THRESHOLD_DAYS = 2;

        [Inject] public UpdatePlayerNotificationCountSignal updatePlayerNotificationCountSignal { get; set; }
        [Inject] public RemoveRecentlyPlayedSignal removeRecentlyPlayedSignal { get; set; }

        public void SortFriends()
        {
            if (inSearchView)
                return;

            // Create holders
            recentlyCompleted = new List<FriendBar>();
            List<FriendBar> activeMatches = new List<FriendBar>();
            List<string> removeRecentCompletedIds = new List<string>();
            List<FriendBar> onlineFbFriends = new List<FriendBar>();
            List<FriendBar> activeFbFriends = new List<FriendBar>();
            List<FriendBar> fbFriends = new List<FriendBar>();

            int notificationCounter = 0;
            notificationTagImage.gameObject.SetActive(false);

            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                LongPlayStatus status = entry.Value.longPlayStatus;
                FriendBar bar = entry.Value;

                if (bar.isCommunity)
                {
                    continue;
                }

                if (status == LongPlayStatus.NEW_CHALLENGE ||
                    status == LongPlayStatus.WAITING_FOR_ACCEPT ||
                    status == LongPlayStatus.PLAYER_TURN ||
                    status == LongPlayStatus.OPPONENT_TURN ||
                    status == LongPlayStatus.DECLINED ||
                    entry.Value.isGameCanceled ||
                    status == LongPlayStatus.PLAYER_WON ||
                    status == LongPlayStatus.OPPONENT_WON ||
                    status == LongPlayStatus.DRAW)
                {
                    activeMatches.Add(bar);

                }
                else if ((bar.friendInfo.flagMask & (long)FriendsFlagMask.RECENT_PLAYED) != 0 &&
                        (bar.lastMatchTimeStamp > 0) &&
                        (bar.lastMatchTimeStamp > (TimeUtil.unixTimestampMilliseconds - (RECENTLY_COMPLETED_THRESHOLD_DAYS * 24 * 60 * 60 * 1000))) &&
                        status == LongPlayStatus.DEFAULT)
                {
                    bar.UpdatePlayButtonStatus(true, localizationService);
                    recentlyCompleted.Add(bar);
                }
                else if (bar.friendType != Friend.FRIEND_TYPE_COMMUNITY)
                {
                    if (entry.Value.isOnline)
                    {
                        onlineFbFriends.Add(bar);
                    }
                    else if (!entry.Value.isOnline && entry.Value.isActive)
                    {
                        activeFbFriends.Add(bar);
                    }
                    else
                    {
                        fbFriends.Add(bar);
                    }
                }
                else
                {
                    entry.Value.gameObject.SetActive(false);
                }

                if (bar.isPlayerTurn || bar.longPlayStatus == LongPlayStatus.DRAW || bar.longPlayStatus == LongPlayStatus.NEW_CHALLENGE
                        || bar.longPlayStatus == LongPlayStatus.PLAYER_WON || bar.longPlayStatus == LongPlayStatus.OPPONENT_WON)
                {
                    notificationCounter++;
                }

            }

            if (notificationCounter > 0)
            {
                notificationTagImage.gameObject.SetActive(true);
            }

            notificationTagNumber.text = notificationCounter.ToString();
            updatePlayerNotificationCountSignal.Dispatch(notificationCounter);

            // Sort holders
            activeMatches.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            recentlyCompleted.Sort((x, y) => -1 * x.lastMatchTimeStamp.CompareTo(y.lastMatchTimeStamp));
            onlineFbFriends.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            activeFbFriends.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            fbFriends.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            // Set sibling indexes
            int index = 0;

            if (activeMatches.Count > 0)
            {
                int countA = 0;
                int maxCount = activeMatches.Count;
                sectionActiveMatches.gameObject.SetActive(true);
                index = sectionActiveMatches.GetSiblingIndex() + 1;
                foreach (FriendBar bar in activeMatches)
                {
                    bar.gameObject.SetActive(true);
                    bar.transform.SetSiblingIndex(index);
                    index++;
                    countA++;
                    bar.UpdateMasking(maxCount == countA, false);
                }
            }
            else
            {
                sectionActiveMatches.gameObject.SetActive(false);
            }

            if (recentlyCompleted.Count > 0)
            {
                int maxCount = settingsModel.maxRecentlyCompletedMatchCount;
                sectionRecentlyCompletedMatches.gameObject.SetActive(true);
                index = sectionRecentlyCompletedMatches.GetSiblingIndex() + 1;
                for (int i = 0; i < recentlyCompleted.Count; i++)
                {
                    if (i < maxCount)
                    {
                        recentlyCompleted[i].gameObject.SetActive(true);
                        recentlyCompleted[i].transform.SetSiblingIndex(index);
                        recentlyCompleted[i].removeCommunityFriendButton.gameObject.SetActive(true);
                        index++;
                        recentlyCompleted[i].UpdateMasking((maxCount == (i + 1) || recentlyCompleted.Count == (i + 1)), false);
                    }
                    else
                    {
                        recentlyCompleted[i].gameObject.SetActive(false);
                        removeRecentCompletedIds.Add(recentlyCompleted[i].friendInfo.playerId);
                    }
                }
            }
            else
            {
                sectionRecentlyCompletedMatches.gameObject.SetActive(false);
            }

            if (removeRecentCompletedIds.Count > 0)
            {
                FriendsSubOp friendsSubOp = new FriendsSubOp(removeRecentCompletedIds, FriendsSubOp.SubOpType.REMOVE_RECENT);

                removeRecentlyPlayedSignal.Dispatch("", friendsSubOp);
            }

            sectionPlayAFriendEmpty.gameObject.SetActive(false);
            sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(false);

            int count = 0;

            sectionPlayAFriend.gameObject.SetActive(true);

            if (!(facebookService.isLoggedIn() || signInWithAppleService.IsSignedIn()))
            {
                index = sectionPlayAFriend.GetSiblingIndex() + 1;
                sectionPlayAFriendEmptyNotLoggedIn.transform.SetSiblingIndex(index);
                sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(true);
                count++;
                index++;
            }
            else
            {
                index = sectionPlayAFriend.GetSiblingIndex() + 1;
            }

            int fbFriendCount = fbFriends.Count + onlineFbFriends.Count + activeFbFriends.Count;
            playerModel.playerFriendsCount = fbFriendCount;

            if (fbFriends.Count > 0 || onlineFbFriends.Count > 0 || activeFbFriends.Count > 0)
            {
                foreach (FriendBar bar in onlineFbFriends)
                {
                    bar.transform.SetSiblingIndex(index);
                    count++;
                    index++;
                    bar.UpdateMasking(count == fbFriendCount, true);
                }

                foreach (FriendBar bar in activeFbFriends)
                {
                    bar.transform.SetSiblingIndex(index);
                    count++;
                    index++;
                    bar.UpdateMasking(count == fbFriendCount, true);
                }

                foreach (FriendBar bar in fbFriends)
                {
                    bar.transform.SetSiblingIndex(index);
                    count++;
                    index++;
                    bar.UpdateMasking(count == fbFriendCount, true);
                }
            }
            else
            {
                if (facebookService.isLoggedIn() || signInWithAppleService.IsSignedIn())
                {
                    sectionPlayAFriendEmpty.transform.SetSiblingIndex(index);
                    sectionPlayAFriendEmpty.gameObject.SetActive(true);
                    count++;
                    index++;
                }

            }
        }

        public void SortSearched(bool isSuccess)
        {
            // Create holders
            List<FriendBar> searchedOnline = new List<FriendBar>();
            List<FriendBar> searchedOffline = new List<FriendBar>();

            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                FriendBar bar = entry.Value;

                if (!bar.isSearched)
                {
                    continue;
                }

                if (entry.Value.isOnline)
                {
                    searchedOnline.Add(bar);
                }
                else
                {
                    searchedOffline.Add(bar);
                }
            }

            // Sort holders
            searchedOnline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            searchedOffline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            // Set sibling indexes
            int index = 0;
            int friendsCount = searchedOnline.Count+ searchedOffline.Count;
            int count = 0;
            if (searchedOnline.Count > 0 || searchedOffline.Count > 0)
            {
                sectionSearched.gameObject.SetActive(true);
                index = sectionSearched.GetSiblingIndex() + 1;

                foreach (FriendBar bar in searchedOnline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                    count++;
                    bar.UpdateMasking(friendsCount==count,true);
                }

                foreach (FriendBar bar in searchedOffline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                    count++;
                    bar.UpdateMasking(friendsCount == count, true);
                }

                //As on server Settings.Friends.searchPageMax = 8
                if (count < 8)  
                {
                    nextSearchButton.interactable = false;
                    nextSearchButtonText.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                    nextSearchButtonTextUnderline.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                }
                else
                {
                    nextSearchButton.interactable = true;
                    nextSearchButtonText.color = Colors.ColorAlpha(Colors.WHITE, Colors.ENABLED_TEXT_ALPHA);
                    nextSearchButtonTextUnderline.color = Colors.ColorAlpha(Colors.WHITE, Colors.ENABLED_TEXT_ALPHA);
                    searchSkip += searchedOnline.Count + searchedOffline.Count;
                }
                
            }
            else
            {
                if (cancelSearchButton.interactable)
                {
                    sectionSearched.gameObject.SetActive(true);
                    sectionSearchResultsEmpty.gameObject.SetActive(true);
                    nextSearchButton.interactable = false;
                    nextSearchButtonText.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                    nextSearchButtonTextUnderline.color = Colors.ColorAlpha(Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
                }
            }

            uiBlocker.gameObject.SetActive(false);
        }

        /*
        void AddTestBars()
        {
            FriendBar cloneSource = null;

            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                cloneSource = entry.Value;
                break;
            }

            cloneSource.isCommunity = false;
            int siblingIndex = friendsSibling.GetSiblingIndex() + 1;

            // new challenges
            FriendBar clone;
            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Clone1";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
            clone.lastActionTime = DateTime.Now.AddDays(-5);
            clone.profileNameLabel.text = "Clone2";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
            clone.lastActionTime = DateTime.Now.AddDays(-3);
            clone.profileNameLabel.text = "Clone3";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            // your move
            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.PLAYER_TURN;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Clone4";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.PLAYER_TURN;
            clone.lastActionTime = DateTime.Now.AddDays(-5);
            clone.profileNameLabel.text = "Clone5";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.PLAYER_TURN;
            clone.lastActionTime = DateTime.Now.AddDays(-3);
            clone.profileNameLabel.text = "Clone6";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);


            // their move

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.OPPONENT_TURN;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Clone7";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.OPPONENT_TURN;
            clone.lastActionTime = DateTime.Now.AddDays(-5);
            clone.profileNameLabel.text = "Clone8";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.OPPONENT_TURN;
            clone.lastActionTime = DateTime.Now.AddDays(-3);
            clone.profileNameLabel.text = "Clone9";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            // ended
            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.DECLINED;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Clone10";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.DRAW;
            clone.lastActionTime = DateTime.Now.AddDays(-2);
            clone.profileNameLabel.text = "Clone11";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.PLAYER_WON;
            clone.lastActionTime = DateTime.Now.AddDays(-5);
            clone.profileNameLabel.text = "Clone12";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.OPPONENT_WON;
            clone.lastActionTime = DateTime.Now.AddDays(-3);
            clone.profileNameLabel.text = "Clone13";
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            // empty

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.DEFAULT;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Zara";
            clone.friendInfo = new Friend();
            clone.friendInfo.publicProfile = new PublicProfile();
            clone.friendInfo.publicProfile.name = clone.profileNameLabel.text;
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.DEFAULT;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Abe";
            clone.friendInfo = new Friend();
            clone.friendInfo.publicProfile = new PublicProfile();
            clone.friendInfo.publicProfile.name = clone.profileNameLabel.text;
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.DEFAULT;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Mike";
            clone.friendInfo = new Friend();
            clone.friendInfo.publicProfile = new PublicProfile();
            clone.friendInfo.publicProfile.name = clone.profileNameLabel.text;
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

        }
        */

    }
}
