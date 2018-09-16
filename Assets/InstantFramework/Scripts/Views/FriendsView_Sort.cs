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
    public partial class FriendsView : View
    {
        public void Sort()
        {
            // Create holders
            List<FriendBar> newChallenges = new List<FriendBar>();
            List<FriendBar> yourMove = new List<FriendBar>();
            List<FriendBar> theirMove = new List<FriendBar>();
            List<FriendBar> ended = new List<FriendBar>();
            List<FriendBar> empty = new List<FriendBar>();

            // Fill holders
            foreach (KeyValuePair<string, FriendBar> entry in bars)
            {
                LongPlayStatus status = entry.Value.longPlayStatus;
                FriendBar bar = entry.Value;

                if (bar.isCommunity)
                    continue;

                if (status == LongPlayStatus.NEW_CHALLENGE)
                {
                    newChallenges.Add(bar);
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
                    empty.Add(bar);
                }
                    
            }

            // Sort holders
            newChallenges.Sort((x, y) => x.lastActionTime.CompareTo(y.lastActionTime));
            yourMove.Sort((x, y) => x.lastActionTime.CompareTo(y.lastActionTime));
            theirMove.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            ended.Sort((x, y) => -1 * x.lastActionTime.CompareTo(y.lastActionTime));
            empty.Sort((x, y) => x.friendInfo.publicProfile.name.CompareTo(y.friendInfo.publicProfile.name));

            // Set sibling indexes
            int index = friendsSibling.GetSiblingIndex() + 1;

            foreach (FriendBar bar in newChallenges)
            {
                bar.transform.SetSiblingIndex(index);
                index++;
            }

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

            foreach (FriendBar bar in empty)
            {
                bar.transform.SetSiblingIndex(index);
                index++;
            }
        }

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
            clone.longPlayStatus = LongPlayStatus.NONE;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Zara";
            clone.friendInfo = new Friend();
            clone.friendInfo.publicProfile = new PublicProfile();
            clone.friendInfo.publicProfile.name = clone.profileNameLabel.text;
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.NONE;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Abe";
            clone.friendInfo = new Friend();
            clone.friendInfo.publicProfile = new PublicProfile();
            clone.friendInfo.publicProfile.name = clone.profileNameLabel.text;
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

            clone = GameObject.Instantiate(cloneSource);
            clone.longPlayStatus = LongPlayStatus.NONE;
            clone.lastActionTime = DateTime.Now.AddDays(-1);
            clone.profileNameLabel.text = "Mike";
            clone.friendInfo = new Friend();
            clone.friendInfo.publicProfile = new PublicProfile();
            clone.friendInfo.publicProfile.name = clone.profileNameLabel.text;
            bars.Add(clone.profileNameLabel.text, clone);
            clone.gameObject.transform.SetParent(listContainer, false);
            clone.gameObject.transform.SetSiblingIndex(siblingIndex);

        }

    }
}
