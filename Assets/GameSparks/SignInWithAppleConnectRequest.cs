// GameSparks Unity SDK

using System;
using GameSparks.Api.Responses;
using GameSparks.Core;

public class SignInWithAppleConnectRequest : GSTypedRequest<SignInWithAppleConnectRequest, AuthenticationResponse>
{

    public SignInWithAppleConnectRequest() : base("SignInWithAppleConnectRequest")
    {

    }

    public SignInWithAppleConnectRequest(GSInstance instance) : base(instance, "SignInWithAppleConnectRequest")
    {

    }


    protected override GSTypedResponse BuildResponse(GSObject response)
    {
        return new AuthenticationResponse(response);
    }


    public SignInWithAppleConnectRequest SetClientId(String clientId)
    {
        request.AddString("clientId", clientId);
        return this;
    }

    public SignInWithAppleConnectRequest SetAuthorizationCode(String authorizationCode)
    {
        request.AddString("authorizationCode", authorizationCode);
        return this;
    }

    public SignInWithAppleConnectRequest SetDoNotCreateNewPlayer(bool doNotCreateNewPlayer)
    {
        request.AddBoolean("doNotCreateNewPlayer", doNotCreateNewPlayer);
        return this;
    }

    public SignInWithAppleConnectRequest SetDoNotLinkToCurrentPlayer(bool doNotLinkToCurrentPlayer)
    {
        request.AddBoolean("doNotLinkToCurrentPlayer", doNotLinkToCurrentPlayer);
        return this;
    }

    public SignInWithAppleConnectRequest SetErrorOnSwitch(bool errorOnSwitch)
    {
        request.AddBoolean("errorOnSwitch", errorOnSwitch);
        return this;
    }

    public SignInWithAppleConnectRequest SetSegments(GSRequestData segments)
    {
        request.AddObject("segments", segments);
        return this;
    }

    public SignInWithAppleConnectRequest SetSwitchIfPossible(bool switchIfPossible)
    {
        request.AddBoolean("switchIfPossible", switchIfPossible);
        return this;
    }


    public SignInWithAppleConnectRequest SetSyncDisplayName(bool syncDisplayName)
    {
        request.AddBoolean("syncDisplayName", syncDisplayName);
        return this;
    }

}