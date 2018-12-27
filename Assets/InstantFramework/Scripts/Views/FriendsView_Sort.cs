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
                    entry.Value.isGameCanceled)
                {
                    newMatches.Add(bar);
                }
                else if (status == LongPlayStatus.PLAYER_TURN)
                {
                    yourMove.Add(bar);
                }
                else if (status == LongPlayStatus.OPPONENT_TURN)
                {
                    theirMove.Add(bar);
                }
                else if (status == LongPlayStatus.DECLINED ||
                         status == LongPlayStatus.PLAYER_WON ||
                         status == LongPlayStatus.OPPONENT_WON ||
                         status == LongPlayStatus.DRAW)
                {
                    ended.Add(bar);
                }
                else
                {
                    if (entry.Value.isOnline)
                    {
                        emptyOnline.Add(bar);
                    }
                    else
                    {
                        emptyOffline.Add(bar);
                    }
                }

            }

            // Sort holders
            newMatches.Sort((x, y) => x.lastActionTime.CompareTo(y.lastActionTime));
            yourMove.Sort((x, y) => x.lastActionTime.CompareTo(y.lastActionTime));
            theirMove.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            ended.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            emptyOnline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            emptyOffline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            // Set sibling indexes
            int index = 0;
            sectionNewMatches.gameObject.SetActive(false);
            sectionPlayAFriendEmpty.gameObject.SetActive(false);
            sectionPlayAFriendEmptyNotLoggedIn.gameObject.SetActive(false);
            sectionActiveMatchesEmpty.gameObject.SetActive(false);


            if (newMatches.Count > 0)
            {
                sectionNewMatches.gameObject.SetActive(true);
                index = sectionNewMatches.GetSiblingIndex() + 1;

                foreach (FriendBar bar in newMatches)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }
            }

            if (yourMove.Count > 0 ||
                theirMove.Count > 0 ||
                ended.Count > 0)
            {
                sectionActiveMatches.gameObject.SetActive(true);

                index = sectionActiveMatches.GetSiblingIndex() + 1;

                foreach (FriendBar bar in yourMove)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }

                foreach (FriendBar bar in theirMove)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }

                foreach (FriendBar bar in ended)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }
            }
            else
            {
                sectionActiveMatches.gameObject.SetActive(false);
            }

            if (emptyOnline.Count > 0 || emptyOffline.Count > 0)
            {
                index = sectionPlayAFriend.GetSiblingIndex() + 1;

                foreach (FriendBar bar in emptyOnline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }

                foreach (FriendBar bar in emptyOffline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }
            }
            else
            {
                if (facebookService.isLoggedIn())
                {
                    sectionPlayAFriendEmpty.gameObject.SetActive(true);
                }
                else
                {
                    sectionPlayAFriendEmptyNotLoggedIn.SetActive(true);
                }
            }
        }

        public void SortCommunity()
        {
            // Create holders
            List<FriendBar> communityOnline = new List<FriendBar>();
            List<FriendBar> communityOffline = new List<FriendBar>();

            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                FriendBar bar = entry.Value;

                if (!bar.isCommunity)
                {
                    continue;
                }

                if (entry.Value.isOnline)
                {
                    communityOnline.Add(bar);
                }
                else
                {
                    communityOffline.Add(bar);
                }
            }

            // Sort holders
            communityOnline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));
            communityOffline.Sort((x, y) => string.Compare(x.friendInfo.publicProfile.name, y.friendInfo.publicProfile.name, StringComparison.Ordinal));

            // Set sibling indexes
            int index = 0;
            sectionPlaySomeoneNewEmpty.gameObject.SetActive(false);


            if (communityOnline.Count > 0 || communityOffline.Count > 0)
            {
                index = sectionPlaySomeoneNew.GetSiblingIndex() + 1;

                foreach (FriendBar bar in communityOnline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;
                }

                foreach (FriendBar bar in communityOffline)
                {
                    bar.transform.SetSiblingIndex(index);
                    index++;

                }
            }
            else
            {
                sectionPlaySomeoneNewEmpty.gameObject.SetActive(true);
            }
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
