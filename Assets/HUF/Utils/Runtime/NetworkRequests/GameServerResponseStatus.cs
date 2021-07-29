using JetBrains.Annotations;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public enum GameServerResponseStatus
    {
        /// <summary>
        /// Indicates the service performing a request is not initialized.
        /// </summary>
        [PublicAPI]
        NotInitialized,

        /// <summary>
        /// Indicates a successful request.
        /// </summary>
        [PublicAPI]
        Success,

        /// <summary>
        /// Indicates a failed request.
        /// </summary>
        [PublicAPI]
        Failed,
    }
}