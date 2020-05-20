using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using SocialEdge.Communication;
using SocialEdge.Requests;

namespace SocialEdge {}

    public class SocialEdgeSDK
    {
        
        /*
         * 

        /// <summary>
        /// Initialize and set up the player for this session
        /// </summary>
        /// <param name="appInfo">App Data to send to the backend</param>
        public void Initialize(var appInfo)
        {
            // Login to Playfab
            if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            {
                PlayFabSettings.TitleId = "42";
            }
            var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }



        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Congratulations, you made your first successful API call!");
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogWarning("Something went wrong with your first API call.  :(");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
        */


        /*
            /// <summary>
            /// Performs default guest authentication for a new player
            /// </summary>
            public void AuthenticateGuest()
            {
            }

            /// <summary>
            /// Authenticates a player with Facebook
            /// </summary>
            /// <param name="token">Player's facebook access token</param>
            public void AuthenticateFacebook(string token)
            {
            }

            /// <summary>
            /// Records a change in the player data model onto player cloud data
            /// </summary>
            /// <param name="valuePair">Player's data fields to update</param>
            public void UpdatePlayerData(var valuePair)
            {
            }


            /// <summary>
            /// Updates the player's display name
            /// </summary>
            /// <param name="name">Player's new display name</param>
            public void UpdatePlayerDisplayName(string name)
            {
            }


            /// <summary>
            /// Adds a friend to the player's friends list
            /// </summary>
            /// <param name="id">ID of the friend to add</param>
            public void AddFriend(string id)
            {
            }

            /// <summary>
            /// Removes a friend from the player's friends list
            /// </summary>
            /// <param name="id">ID of the friend to remove</param>
            public void RemoveFriend(string id)
            {
            }

            /// <summary>
            /// Tags a friend as blocked on the player's friends list
            /// </summary>
            /// <param name="id">ID of the friend to block</param>
            public void BlockFriend(string id)
            {
            }

            /// <summary>
            /// Unblocks a friend on the player's friends list
            /// </summary>
            /// <param name="token">ID of the friend to unblock/param>
            public void UnblockFriend(string id)
            {
            }

            /// <summary>
            /// Retrieves list of players from a search result
            /// </summary>
            /// <param name="search">Input string to search in player names</param>
            /// <param name="page">Search results page index</param>
            public void GetSearchList(string search, int page)
            {
            }

            /// <summary>
            /// Retrieves the community players list
            /// </summary>
            public void GetCommunityList()
            {
            }

            /// <summary>
            /// Sends a chat message to another player
            /// </summary>
            /// <param name="id">ID of player to send to</param>
            /// <param name="chat">Text message to send</param>
            public void SendChat(string id, string chat)
            {
            }

            /// <summary>
            /// Registers a player's push notification token
            /// </summary>
            /// <param name="token">Push notification token from the push notification service provider</param>
            public void RegisterPushNotficationToken(string token)
            {
            }

            /// <summary>
            /// Restores a player's purchases
            /// </summary>
            public void RestorePurchases()
            {
            }

            /// <summary>
            /// Verifies a purchase from the store provider
            /// </summary>
            /// <param name="orderId"> Order ID to verify</param>
            public void VerifyPurchase(string orderId)
            {
            }

            /// <summary>
            /// Purchases an item from the shop catalog
            /// </summary>
            /// <param name="itemId">ID of item to purchase</param>
            public void PurchaseItem(string itemId)
            {
            }

            /// <summary>
            /// Sends a game invitation to an opponent
            /// </summary>
            /// <param name="id">Opponent's ID</param>
            /// <param name="invitation" Invitation object </param>
            public void SendGameInvite(string id, var invitation)
            {
            }

            /// <summary>
            /// Retrieves a player's current status
            /// </summary>
            /// <param name="id">ID of the player to probe</param>
            public void GetPlayerStatus(string id)
            {
            }

            /// <summary>
            /// Ping the general purpose pinger
            /// </summary>
            /// <param name="info">Information structure sent to the pinger/param>
            private void Ping(var info)
            {
            }

            /// <summary>
            /// Retrieves sync data and pending messages
            /// </summary>
            public void SyncReconnectData()
            {
            }
        */

        /*
    #region public methods
    public void UpdateDisplayName(string updatedName)
    {
        try
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = updatedName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplayNameSuccess, OnError);

        }
        catch (Exception ex)
        {
            Debug.Log("Exception encountered in UpdateDisplayName due to " + ex.InnerException.ToString());
        }
    }


    public void AddFriend(string id)
    {
        try
        {
            var request = new AddFriendRequest
            {
                FriendPlayFabId = id
            };

            PlayFabClientAPI.AddFriend(request, OnAddFriendSuccess, OnAddFriendError);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception encountered in AddFriend due to " + ex.InnerException.ToString());
        }
    }

    public void RemoveFriend(string id)
    {
        try
        {
            var request = new RemoveFriendRequest
            {
                FriendPlayFabId = id
            };

            PlayFabClientAPI.RemoveFriend(request, OnRemoveFriendSuccess, OnRemoveFriendError);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception encountered in RemoveFriend due to " + ex.InnerException.ToString());
        }
    }

    public void GetSearchList()
    {
        try
        {
            var request = new GetFriendsListRequest();
            PlayFabClientAPI.GetFriendsList(request, OnSearchSuccess, OnSearchError);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception encountered in GetSearchList due to " + ex.InnerException.ToString());
        }
    }

    public void AddFriendTag()
    {
        try
        {
            var request = new SetFriendTagsRequest
            {
                FriendPlayFabId = "1202C3998588AAA7",
                Tags = new List<string> { "blocked" }

            };

            PlayFabClientAPI.SetFriendTags(request, OnAddFriendTagSuccess, OnAddFriendTagFailure);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception encountered in AddFriendTag due to " + ex.InnerException.ToString());
        }

    }

    #endregion

    #region private methods
    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Congratulations, display name updated");
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Display name could not be updated due to " + error.Error.ToString());
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("Congratulations, display name updated");
    }

    private void OnAddFriendError(PlayFabError error)
    {
        Debug.Log("Display name could not be updated due to " + error.Error.ToString());
    }
    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        Debug.Log("Congratulations, display name updated");
    }

    private void OnRemoveFriendError(PlayFabError error)
    {
        Debug.Log("Display name could not be updated due to " + error.Error.ToString());
    }
    private void OnSearchSuccess(GetFriendsListResult result)
    {
        Debug.Log("Congratulations, display name updated");
    }

    private void OnSearchError(PlayFabError error)
    {
        Debug.Log("Display name could not be updated due to " + error.Error.ToString());
    }
    private void OnAddFriendTagSuccess(SetFriendTagsResult result)
    {
        Debug.Log("Congratulations, display name updated");
    }

    private void OnAddFriendTagFailure(PlayFabError error)
    {
        Debug.Log("Display name could not be updated due to " + error.Error.ToString());
    }
    #endregion
    */
    }
