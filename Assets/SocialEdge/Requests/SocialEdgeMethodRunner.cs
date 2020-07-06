using System;
using SocialEdge.Requests;
using UnityEngine;

public class SocialEdgeMethodRunner
{
    private string friendId;
    public SocialEdgeMethodRunner()
    {
        friendId = "BAEADFD1AB871D87";
    }

    public void Run()
    {
        SocialEdge_Login_Success();
    }

    /// <summary>
    /// Set login callback and send login request
    /// </summary>
    public void SocialEdge_Login_Success()
    {
        Action<SocialEdgeBackendLoginResponse> target = LoginSuccessCallBack;
        var req = new SocialEdgeBackendLoginRequest().SetSuccessCallback(target).SetFailureCallback(target).GetBasicInfo();
        req.Send();
        var a = 123;
    }


    public void LoginSuccessCallBack(SocialEdgeBackendLoginResponse resp)
    {
        Debug.Log("login callback");
       // SocialEdge_AddFriend_Success(friendId);
    }

    /// <summary>
    ///  Set add friend success and failure callbacks and send add friend request
    /// </summary>
    public void SocialEdge_AddFriend_Success(string friendUserId)
    {
        Action<SocialEdgeAddFriendResponse> successTarget = AddFriendSuccessCallBack;
        Action<SocialEdgeAddFriendResponse> failureTarge = AddFriendFailureCallBack;


        new SocialEdgeAddFriendRequest().SetFriendUseId(friendUserId)
                                        .SetSuccessCallback(successTarget)
                                        .SetFailureCallback(failureTarge)
                                        .Send();
    }


    public void AddFriendSuccessCallBack(SocialEdgeAddFriendResponse resp)
    {
        Debug.Log("Friend added successfully");
        SocialEdge_RemoveFriend_Success(friendId);
    }

    public void AddFriendFailureCallBack(SocialEdgeAddFriendResponse resp)
    {
        Debug.Log("Friend already added");
        SocialEdge_RemoveFriend_Success(friendId);
    }

    /// <summary>
    ///  Set remove friend callbacks and send remove friend request
    /// </summary>
    public void SocialEdge_RemoveFriend_Success(string friendUserId)
    {
        Action<SocialEdgeRemoveFriendResponse> successTarget = RemoveFriendSuccessCallBack;
        Action<SocialEdgeRemoveFriendResponse> failureTarget = RemoveFriendFailureCallBack;
        new SocialEdgeRemoveFriendRequest().SetFriendUserId(friendUserId)
                                            .SetSuccessCallback(successTarget)
                                            .SetFailureCallback(failureTarget)
                                            .Send();
    }

    public void RemoveFriendSuccessCallBack(SocialEdgeRemoveFriendResponse resp)
    {
        Debug.Log("Friend removed successfully");
        SocialEdge_GetFriends_Success();
    }

    public void RemoveFriendFailureCallBack(SocialEdgeRemoveFriendResponse resp)
    {
        Debug.Log("I am not friends with this user >.<");
        SocialEdge_GetFriends_Success();
    }

    /// <summary>
    /// Set block friend callback and send block friend request
    /// </summary>
    public void SocialEdge_BlockFriend_Success()
    {
        Action<SocialEdgeBlockFriendResponse> target = BlockFriendSuccessCallBack;
        new SocialEdgeBlockFriendRequest().SetSuccessCallback(target)
                 .Send();
    }

    
    public void BlockFriendSuccessCallBack(SocialEdgeBlockFriendResponse resp)
    {
        Debug.Log("friend blocked successfully");
    }

    /// <summary>
    /// Set unblock friend callback and send block friend request
    /// </summary>
    public void SocialEdge_UnblockFriend_Success()
    {
        Action<SocialEdgeUnblockFriendResponse> target = UnblockFriendSuccessCallBack;
        new SocialEdgeUnblockFriendRequest().SetSuccessCallback(target)
                 .Send();
    }

    
    public void UnblockFriendSuccessCallBack(SocialEdgeUnblockFriendResponse resp)
    {
        Debug.Log("friend unblocked successfully");
    }

    /// <summary>
    /// Set get friends callback and send get friends request
    /// </summary>
    public void SocialEdge_GetFriends_Success()
    {
        Action<SocialEdgeGetFriendsResponse> target = GetFriendsSuccessCallBack;
        new SocialEdgeGetFriendsRequest().SetSuccessCallback(target)
                 .Send();
    }

    public void GetFriendsSuccessCallBack(SocialEdgeGetFriendsResponse resp)
    {
        Debug.Log("friends searched successfully");
    }
}
