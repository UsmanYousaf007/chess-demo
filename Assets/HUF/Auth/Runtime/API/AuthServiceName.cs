using JetBrains.Annotations;

namespace HUF.Auth.API
{
    /// <summary>
    /// Authentication services names. <para />
    /// Could be used in HAuth methods if there is more services registered in one time.
    /// </summary>
    [PublicAPI]
    public static class AuthServiceName
    {
        public const string FIREBASE = "firebase";
        public const string FACEBOOK = "facebook";
        public const string SIWA = "sign_in_with_apple";
        public const string GPGS = "google_play_game_services";
    }
}