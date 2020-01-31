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
        public void SortFriends()
        {
            // Create holders
            List<FriendBar> newMatches = new List<FriendBar>();
            List<FriendBar> yourMove = new List<FriendBar>();
            List<FriendBar> theirMove = new List<FriendBar>();
            List<FriendBar> ended = new List<FriendBar>();
            List<FriendBar> emptyOnline = new List<FriendBar>();
            List<FriendBar> emptyOffline = new List<FriendBar>();
            List<FriendBar> allFriends = new List<FriendBar>();

            List<FriendBar> onlineFbFriends = new List<FriendBar>();
            List<FriendBar> fbFriends = new List<FriendBar>();
            List<FriendBar> OnlineRecentCompleted = new List<FriendBar>();
            List<FriendBar> recentCompleted = new List<FriendBar>();

            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                LongPlayStatus status = entry.Value.longPlayStatus;
                FriendBar bar = entry.Value;

                if (bar.isCommunity)
                {
                    continue;
                }

                if(bar.friendType == Friend.FRIEND_TYPE_COMMUNITY)
                {
                    if (status != LongPlayStatus.DEFAULT)
                    {
                        bar.gameObject.SetActive(false);
                        continue;
                    }

                    if (entry.Value.isOnline)
                    {
                        OnlineRecentCompleted.Add(bar);
                    }
                    else
                    {
                        recentCompleted.Add(bar);
                    }
                }
                else //fb friends
                {
                    if (entry.Value.isOnline)
                    {
                        onlineFbFriends.Add(bar);
                    }
                    else
                    {
                        fbFriends.Add(bar);
                    }

                }

                
                

                //if (status == LongPlayStatus.NEW_CHALLENGE ||
                //    status == LongPlayStatus.WAITING_FOR_ACCEPT ||
                //    entry.Value.isGameCanceled)
                //{
                //    newMatches.Add(bar);
                //}
                //else if (status == LongPlayStatus.PLAYER_TURN)
                //{
                //    yourMove.Add(bar);
                //}
                //else if (status == LongPlayStatus.OPPONENT_TURN)
                //{
                //    theirMove.Add(bar);
                //}
                //else if (status == LongPlayStatus.DECLINED ||
                //         status == LongPlayStatus.PLAYER_WON ||
                //         status == LongPlayStatus.OPPONENT_WON ||
                //         status == LongPlayStatus.DRAW)
                //{
                //    ended.Add(bar);
                //}
                //else
                //{
                //    if (entry.Value.isOnline)
                //    {
                //        emptyOnline.Add(bar);
                //    }
                //    else
                //    {
                //        emptyOffline.Add(bar);
                //    }
                //}
            }

            // Sort holders
            //newMatches.Sort((x, y) => x.lastActionTime.CompareTo(y.lastActionTime));
            //yourMove.Sort((x, y) => x.lastActionTime.CompareTo(y.lastActionTime));
            //theirMove.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            //ended.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            //emptyOnline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            //emptyOffline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            onlineFbFriends.Sort((x, y) => -1 * x.lastMatchTimeStamp.CompareTo(y.lastMatchTimeStamp));
            fbFriends.Sort((x, y) => -1 * x.lastMatchTimeStamp.CompareTo(y.lastMatchTimeStamp));
            OnlineRecentCompleted.Sort((x, y) => -1 * x.lastMatchTimeStamp.CompareTo(y.lastMatchTimeStamp));
            recentCompleted.Sort((x, y) => -1 * x.lastMatchTimeStamp.CompareTo(y.lastMatchTimeStamp));

            //allFriends.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal)); 

            // Set sibling indexes
            int index = 0;
            
            sectionPlayAFriendEmpty.gameObject.SetActive(false);
            sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(false);
            sectionRecentlyCompleted.gameObject.SetActive(false);

            int count = 0;

            if (!inSearchView)
            {
                sectionPlayAFriend.gameObject.SetActive(true);



                if (!facebookService.isLoggedIn())
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
            }



            //if (yourMove.Count > 0 ||
            //    theirMove.Count > 0 ||
            //    ended.Count > 0 ||
            //    newMatches.Count >0 ||
            //    emptyOnline.Count > 0 ||
            //    emptyOffline.Count > 0
            //    )

            int fbFriendCount = fbFriends.Count + onlineFbFriends.Count;
            playerModel.playerFriendsCount = fbFriendCount;

            if (fbFriends.Count > 0 || onlineFbFriends.Count > 0)
            {
                foreach (FriendBar bar in onlineFbFriends)
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
                if (facebookService.isLoggedIn())
                {
                    sectionPlayAFriendEmpty.transform.SetSiblingIndex(index);
                    sectionPlayAFriendEmpty.gameObject.SetActive(true);
                    count++;
                    index++;
                }
                    
            }


            int recentMatchCount = OnlineRecentCompleted.Count + recentCompleted.Count;
            if (OnlineRecentCompleted.Count > 0 || recentCompleted.Count > 0)
            {
                if(!inSearchView)
                    sectionRecentlyCompleted.gameObject.SetActive(true);

                index = sectionRecentlyCompleted.GetSiblingIndex() + 1;

                foreach (FriendBar bar in OnlineRecentCompleted)
                {
                    bar.transform.SetSiblingIndex(index);
                    count++;
                    index++;
                    bar.UpdateMasking(count == recentMatchCount, true);
                }

                foreach (FriendBar bar in recentCompleted)
                {
                    bar.transform.SetSiblingIndex(index);
                    count++;
                    index++;
                    bar.UpdateMasking(count == recentMatchCount, true);
                }
                //foreach (FriendBar bar in yourMove)
                //{
                //    bar.transform.SetSiblingIndex(index);
                //    index++;
                //}

                //foreach (FriendBar bar in newMatches)
                //{
                //    bar.transform.SetSiblingIndex(index);
                //    index++;
                //}

                //foreach (FriendBar bar in theirMove)
                //{
                //    bar.transform.SetSiblingIndex(index);
                //    index++;
                //}

                //foreach (FriendBar bar in ended)
                //{
                //    bar.transform.SetSiblingIndex(index);
                //    index++;
                //}

                //foreach (FriendBar bar in emptyOnline)
                //{
                //    bar.transform.SetSiblingIndex(index);
                //    index++;
                //}

                //foreach (FriendBar bar in emptyOffline)
                //{
                //    bar.transform.SetSiblingIndex(index);
                //    index++;
                //}
            }

            //if (emptyOnline.Count > 0 || emptyOffline.Count > 0)
            //{
            //    index = sectionPlayAFriend.GetSiblingIndex() + 1;

            //    foreach (FriendBar bar in emptyOnline)
            //    {
            //        bar.transform.SetSiblingIndex(index);
            //        index++;
            //    }

            //    foreach (FriendBar bar in emptyOffline)
            //    {
            //        bar.transform.SetSiblingIndex(index);
            //        index++;
            //    }
            //}
            //else
            //{
            //    if (facebookService.isLoggedIn())
            //    {
            //        sectionPlayAFriendEmpty.gameObject.SetActive(true);
            //    }
            //}
        }

        public void SortSearched()
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
