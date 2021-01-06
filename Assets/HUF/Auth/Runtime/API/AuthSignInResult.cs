namespace HUF.Auth.Runtime.API
{
    public enum AuthSignInResult
    {
        Success = 0,
        Cancelled,
        ConnectionFailure,
        NotAuthenticated,
        UnspecifiedFailure = 1000,
    }
}