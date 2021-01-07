using System;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using UnityEngine;
namespace HUF.AuthSIWA.Runtime.Implementation
{
    public enum UserDetectionStatus
    {
        LikelyReal,
        Unknown,
        Unsupported
    }
    [Serializable]
    public struct UserInfo
    {
        public string userId;
        public string email;
        public string displayName;
        public string idToken;
        public string error;
        public string authorizationCode;
        public UserDetectionStatus userDetectionStatus;
        public static UserInfo BuildFromIAppleIDCredential( IAppleIDCredential credential )
        {
            string BytesToString( byte[] b ) => System.Text.Encoding.UTF8.GetString( b, 0, b.Length );
            var userInfo = new UserInfo
            {
                userId = credential.User,
                email = credential.Email,
                displayName = credential.FullName != null ? credential.FullName.ToLocalizedString() : string.Empty,
                idToken = BytesToString( credential.IdentityToken ),
                authorizationCode = BytesToString( credential.AuthorizationCode )
            };
            switch ( credential.RealUserStatus )
            {
                case RealUserStatus.Unsupported:
                    userInfo.userDetectionStatus = UserDetectionStatus.Unsupported;
                    break;
                case RealUserStatus.Unknown:
                    userInfo.userDetectionStatus = UserDetectionStatus.Unknown;
                    break;
                case RealUserStatus.LikelyReal:
                    userInfo.userDetectionStatus = UserDetectionStatus.LikelyReal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return userInfo;
        }
    }
}
