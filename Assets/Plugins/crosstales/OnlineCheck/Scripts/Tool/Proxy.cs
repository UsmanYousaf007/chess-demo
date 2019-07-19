using UnityEngine;

namespace Crosstales.OnlineCheck.Tool
{
    /// <summary>Handles HTTP/HTTPS Internet connections via proxy server.</summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [HelpURL("https://crosstales.com/media/data/assets/OnlineCheck/api/class_crosstales_1_1_online_check_1_1_tool_1_1_proxy.html")]
    public class Proxy : MonoBehaviour
    {

        #region Variables

        [Header("HTTP Proxy Settings")]
        /// <summary>URL (without protocol) or IP of the proxy server.</summary>
        [Tooltip("URL (without protocol) or IP of the proxy server.")]
        public string HTTPProxyURL;

        /// <summary>Port of the proxy server.</summary>
        [Tooltip("Port of the proxy server.")]
        [Range(0, 65535)]
        public int HTTPProxyPort = 8080;

        /// <summary>Username for the proxy server (optional).</summary>
        [Tooltip("Username for the proxy server (optional).")]
        public string HTTPProxyUsername = string.Empty;

        /// <summary>Password for the proxy server (optional).</summary>
        [Tooltip("Password for the proxy server (optional).")]
        public string HTTPProxyPassword = string.Empty;

        /// <summary>Protocol (e.g. 'http://') for the proxy server (optional).</summary>
        [Tooltip("Protocol (e.g. 'http://') for the proxy server (optional).")]
        public string HTTPProxyURLProtocol = string.Empty;

        [Header("HTTPS Proxy Settings")]
        /// <summary>URL (without protocol) or IP of the proxy server.</summary>
        [Tooltip("URL (without protocol) or IP of the proxy server.")]
        public string HTTPSProxyURL;

        /// <summary>Port of the proxy server.</summary>
        [Tooltip("Port of the proxy server.")]
        [Range(0, 65535)]
        public int HTTPSProxyPort = 8443;

        /// <summary>Username for the proxy server (optional).</summary>
        [Tooltip("Username for the proxy server (optional).")]
        public string HTTPSProxyUsername = string.Empty;

        /// <summary>Password for the proxy server (optional).</summary>
        [Tooltip("Password for the proxy server (optional).")]
        public string HTTPSProxyPassword = string.Empty;

        /// <summary>Protocol (e.g. 'http://') for the proxy server (optional).</summary>
        [Tooltip("Protocol (e.g. 'http://') for the proxy server (optional).")]
        public string HTTPSProxyURLProtocol = string.Empty;

        [Header("Startup Behaviour")]
        /// <summary>Enable the proxy on awake (default: false).</summary>
        [Tooltip("Enable the proxy on awake (default: false).")]
        public bool EnableOnAwake = false;

        private const string HTTPProxyEnvVar = "HTTP_PROXY";
        private const string HTTPSProxyEnvVar = "HTTPS_PROXY";

        private static bool httpProxy = false;
        private static bool httpsProxy = false;

        #endregion


        #region Properties

        /// <summary>Is HTTP-proxy enabled?</summary>
        /// <returns>True if the HTTP-proxy is enabled.</returns>
        public static bool hasHTTPProxy
        {
            get
            {
                return httpProxy;
            }
        }

        /// <summary>Is HTTPS-proxy enabled?</summary>
        /// <returns>True if the HTTPS-proxy is enabled.</returns>
        public static bool hasHTTPSProxy
        {
            get
            {
                return httpsProxy;
            }
        }

        #endregion


        #region MonoBehaviour methods

        public void Awake()
        {
            if (EnableOnAwake)
            {
                EnableHTTPProxy();
                EnableHTTPSProxy();
            }
        }

        public void Update()
        {
            if (Util.Helper.isEditorMode)
            {
                name = Util.Constants.PROXY_SCENE_OBJECT_NAME; //ensure name
            }
        }

        #endregion


        #region Public methods

        /// <summary>Enables or disables a proxy server for HTTPS connections with the current instance variables as parameters.</summary>
        /// <param name="enabled">Enable the proxy server (default: true, optional)</param>
        public void EnableHTTPProxy(bool enabled = true)
        {
            if (enabled)
            {
                EnableHTTPProxy(HTTPProxyURL, HTTPProxyPort, HTTPProxyUsername, HTTPProxyPassword, HTTPProxyURLProtocol);
            }
            else
            {
                DisableHTTPProxy();
            }
        }

        /// <summary>Enables or disables a proxy server for HTTPS connections with the current instance variables as parameters.</summary>
        /// <param name="enabled">Enable the proxy server (default: true, optional)</param>
        public void EnableHTTPSProxy(bool enabled = true)
        {
            if (enabled)
            {
                EnableHTTPSProxy(HTTPSProxyURL, HTTPSProxyPort, HTTPSProxyUsername, HTTPSProxyPassword, HTTPSProxyURLProtocol);
            }
            else
            {
                DisableHTTPSProxy();
            }
        }

        /// <summary>Enables or disables a proxy server for HTTP connections.</summary>
        /// <param name="url">URL (without protocol) or IP of the proxy server</param>
        /// <param name="port">Port of the proxy server</param>
        /// <param name="username">"Username for the proxy server (optional)</param>
        /// <param name="password">Password for the proxy server (optional)</param>
        /// <param name="urlProtocol">Protocol (e.g. 'http://') for the proxy server (optional)</param>
        public void EnableHTTPProxy(string url, int port, string username = "", string password = "", string urlProtocol = "")
        {
#if UNITY_STANDALONE || (UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX))
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("HTTP proxy url cannot be null or empty!");
                return;
            }
            if (!validPort(port))
            {
                Debug.LogError("HTTP proxy port must be valid (between 0 - 65535): " + port);
                return;
            }

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                System.Environment.SetEnvironmentVariable(HTTPProxyEnvVar, urlProtocol + username + ":" + password + "@" + url + ":" + port);
            }
            else
            {
                System.Environment.SetEnvironmentVariable(HTTPProxyEnvVar, urlProtocol + url + ":" + port);
            }

            httpProxy = true;
#endif
        }

        /// <summary>Enables or disables a proxy server for HTTPS connections.</summary>
        /// <param name="url">URL (without protocol) or IP of the proxy server</param>
        /// <param name="port">Port of the proxy server</param>
        /// <param name="username">"Username for the proxy server (optional)</param>
        /// <param name="password">Password for the proxy server (optional)</param>
        /// <param name="urlProtocol">Protocol (e.g. 'http://') for the proxy server (optional)</param>
        public void EnableHTTPSProxy(string url, int port, string username = "", string password = "", string urlProtocol = "")
        {
#if UNITY_STANDALONE || (UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX))
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("HTTPS proxy url cannot be null or empty!");
                return;
            }
            if (!validPort(port))
            {
                Debug.LogError("HTTPS proxy port must be valid (between 0 - 65535): " + port);
                return;
            }

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                System.Environment.SetEnvironmentVariable(HTTPSProxyEnvVar, urlProtocol + username + ":" + password + "@" + url + ":" + port);
            }
            else
            {
                System.Environment.SetEnvironmentVariable(HTTPSProxyEnvVar, urlProtocol + url + ":" + port);
            }

            httpsProxy = true;
#endif
        }

        /// <summary>Disables the proxy server for HTTP connections.</summary>
        public void DisableHTTPProxy()
        {
#if UNITY_STANDALONE || (UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX))
            System.Environment.SetEnvironmentVariable(HTTPProxyEnvVar, string.Empty);

            httpProxy = false;
#endif
        }

        /// <summary>Disables the proxy server for HTTPS connections.</summary>
        public void DisableHTTPSProxy()
        {
#if UNITY_STANDALONE || (UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX))
            System.Environment.SetEnvironmentVariable(HTTPSProxyEnvVar, string.Empty);

            httpsProxy = false;
#endif
        }

        #endregion


        #region Private methods

        private static bool validPort(int port)
        {
            return port >= 0 && port < 65536;
        }

        #endregion
    }
}
// © 2016-2019 crosstales LLC (https://www.crosstales.com)