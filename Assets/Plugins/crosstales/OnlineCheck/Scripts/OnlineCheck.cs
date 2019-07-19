using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Crosstales.OnlineCheck
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [HelpURL("https://crosstales.com/media/data/assets/OnlineCheck/api/class_crosstales_1_1_online_check_1_1_online_check.html")]
    /// <summary>Checks the Internet availabilty.</summary>
    public class OnlineCheck : MonoBehaviour
    {

        #region Variables

        [Header("General Settings")]
        /// <summary>Continuously check for Internet availability within given intervals (default: true).</summary>
        [Tooltip("Continuously check for Internet availability within given intervals (default: true).")]
        public bool EndlessMode = true;

        /// <summary>Minimum delay between checks in seconds (default: 3, range: 3 - 120).</summary>
        [Tooltip("Minimum delay between checks in seconds (default: 3, range: 3 - 120).")]
        [Range(3, 119)]
        public int IntervalMin = 4;

        /// <summary>Maximum delay between checks in seconds (default: 10, range: 4 - 120).</summary>
        [Tooltip("Maximum delay between checks in seconds (default: 10, range: 4 - 120).")]
        [Range(4, 120)]
        public int IntervalMax = 10;

        /// <summary>Timeout for every check in seconds (default: 2, range: 1 - 20).</summary>
        [Tooltip("Timeout for every check in seconds (default: 2, range: 1 - 20).")]
        [Range(1, 20)]
        public int Timeout = 2;

        /// <summary>Force UnityWebRequest instead of WebClient (default: false).</summary>
        [Tooltip("Force UnityWebRequest instead of WebClient (default: false).")]
        public bool ForceWWW = false;

        [Tooltip("Use a custom configuration for the checks.")]
        [ContextMenuItem("Create CustomCheck", "createCustomCheck")]
        public Data.CustomCheck CustomCheck;

        [Header("Behaviour Settings")]
        /// <summary>Start at runtime (default: true).</summary>
        [Tooltip("Start at runtime (default: true).")]
        public bool RunOnStart = true;

        /// <summary>Delay in seconds until the OnlineCheck starts checking (default: 0).</summary>
        [Tooltip("Delay in seconds until the OnlineCheck starts checking (default: 0).")]
        public float Delay = 0f;

        /// <summary>Don't destroy gameobject during scene switches (default: true).</summary>
        [Tooltip("Don't destroy gameobject during scene switches (default: true).")]
        public bool DontDestroy = true;

        private bool isFirsttime = true;
        private bool lastInternetAvailable = false;
        private NetworkReachability lastNetworkReachability;

        private bool isRunning = false;

        private float checkTime = 9999f;
        private float checkTimeCounter = 0f;

        private float burstTime = 9999f;
        private float burstTimeCounter = 0f;
        private bool available = false;

        private static bool internetAvailable = false;
        private static NetworkReachability networkReachability;

        private static GameObject go;
        private static OnlineCheck instance;
        private static bool loggedOnlyOneInstance = false;
        private static long lastCheckTime = long.MinValue;

        private const float burstIntervalMin = 2f;
        private const float burstIntervalMax = 5f;

        private const string microsoftUrl = "http://www.msftncsi.com/ncsi.txt";
        private const string appleUrl = "https://www.apple.com/library/test/success.html";
        private const string ubuntutUrl = "https://start.ubuntu.com/connectivity-check";
        private const string fallbackUrl = "https://crosstales.com/media/downloads/up.txt";

        private const string microsoftText = "Microsoft NCSI";
        private const string appleText = "<TITLE>Success</TITLE>";
        private const string ubuntuText = "<TITLE>Lorem Ipsum</TITLE>";
        private const string fallbackText = "crosstales rulez!";

        private const string windowsDesc = "Microsoft";
        private const string appleDesc = "Apple";
        private const string ubuntuDesc = "Ubuntu";
        private const string fallbackDesc = "fallback (crosstales)";
        private const string customDesc = "custom URL";
        private const string testingDesc = "] Testing the Internet availability with ";

        private const bool microsoftEquals = true;
        private const bool appleEquals = false;
        private const bool ubuntuEquals = false;
        private const bool fallbackEquals = true;

#if !UNITY_WSA || UNITY_EDITOR
        private System.Threading.Thread worker;
#endif

        #endregion


        #region Events

        /// <summary>Callback to determine whether the online status has changed or not.</summary>
        public delegate void OnlineStatusChange(bool isConnected);

        /// <summary>Callback to determine whether the network reachability has changed or not.</summary>
        public delegate void NetworkReachabilityChange(NetworkReachability networkReachability);

        /// <summary>Callback to determine whether the checks have completed or not.</summary>
        public delegate void OnlineCheckComplete(bool isConnected, NetworkReachability networkReachability);

        /// <summary>An event triggered whenever the Internet connection status changes.</summary>
        public static event OnlineStatusChange OnOnlineStatusChange
        {
            add { _onOnlineStatusChange += value; }
            remove { _onOnlineStatusChange -= value; }
        }

        /// <summary>An event triggered whenever the network reachability changes.</summary>
        public static event NetworkReachabilityChange OnNetworkReachabilityChange
        {
            add { _onNetworkReachabilityChange += value; }
            remove { _onNetworkReachabilityChange -= value; }
        }

        /// <summary>An event triggered whenever the Internet connection check is completed.</summary>
        public static event OnlineCheckComplete OnOnlineCheckComplete
        {
            add { _onOnlineCheckComplete += value; }
            remove { _onOnlineCheckComplete -= value; }
        }

        private static OnlineStatusChange _onOnlineStatusChange;
        private static NetworkReachabilityChange _onNetworkReachabilityChange;
        private static OnlineCheckComplete _onOnlineCheckComplete;

        #endregion


        #region Properties

        /// <summary>Continuously check for Internet availability within given intervals.</summary>
        public static bool isEndlessMode
        {
            get
            {
                if (instance != null)
                {
                    return instance.EndlessMode;
                }

                return true;
            }

            set
            {
                if (instance != null)
                {
                    instance.EndlessMode = value;
                }
            }
        }

        /// <summary>Minimum delay between checks in seconds (default: 3, range: 3 - 120).</summary>
        public static int CheckIntervalMin
        {
            get
            {
                if (instance != null)
                {
                    if (instance.IntervalMin > instance.IntervalMax)
                    {
                        instance.IntervalMin = instance.IntervalMax - 1;
                    }

                    return instance.IntervalMin;
                }

                return 3;
            }

            set
            {
                if (instance != null)
                {
                    int number = Mathf.Clamp(value, 3, 120);
                    instance.IntervalMin = number < instance.IntervalMax ? number : instance.IntervalMax - 1;
                }
            }
        }

        /// <summary>Maximum delay between checks in seconds (default: 10, range: 4 - 120).</summary>
        public static int CheckIntervalMax
        {
            get
            {
                if (instance != null)
                {
                    if (instance.IntervalMax < instance.IntervalMin)
                    {
                        instance.IntervalMax = instance.IntervalMin + 1;
                    }

                    return instance.IntervalMax;
                }

                return 10;
            }

            set
            {
                if (instance != null)
                {
                    int number = Mathf.Clamp(value, 4, 120);
                    instance.IntervalMax = number > instance.IntervalMin ? number : instance.IntervalMin + 1;
                }
            }
        }

        /// <summary>Timeout for every check in seconds (default: 2, range: 1 - 20).</summary>
        public static int CheckTimeout
        {
            get
            {
                if (instance != null)
                {
                    if (instance.Timeout >= instance.IntervalMin)
                    {
                        instance.Timeout = instance.IntervalMin - 1;
                    }

                    return instance.Timeout;
                }

                return 2;
            }

            set
            {
                if (instance != null)
                {
                    int number = Mathf.Clamp(value, 1, 20);
                    instance.Timeout = number < instance.IntervalMin ? number : instance.IntervalMin - 1;
                }
            }
        }

        /// <summary>Force UnityWebRequest instead of WebClient.</summary>
        public static bool isForceWWW
        {
            get
            {
                if (instance != null)
                {
                    return instance.ForceWWW;
                }

                return false;
            }

            set
            {
                if (instance != null)
                {
                    instance.ForceWWW = value;
                }
            }
        }

        /// <summary>Use a custom configuration for the checks.</summary>
        public static Data.CustomCheck CurrentCustomCheck
        {
            get
            {
                if (instance != null)
                {
                    return instance.CustomCheck;
                }

                return null;
            }

            set
            {
                if (instance != null)
                {
                    instance.CustomCheck = value;
                }
            }
        }

        /// <summary>Returns true if an Internet connection is available.</summary>
        /// <returns>True if an Internet connection is available.</returns>
        public static bool isInternetAvailable
        {
            get
            {
                if (instance == null)
                {
                    return Application.internetReachability != NetworkReachability.NotReachable;
                }
                else
                {
                    return internetAvailable;
                }
            }
        }

        /// <summary>Returns the network reachability.</summary>
        /// <returns>The Internet reachability.</returns>
        public static NetworkReachability NetworkReachability
        {
            get
            {
                if (instance == null)
                {
                    return Application.internetReachability;
                }
                else
                {
                    return networkReachability;
                }
            }
        }

        /// <summary>Returns the time of the last availability check.</summary>
        /// <returns>Time of the last availability check.</returns>
        public static System.DateTime LastCheck
        {
            get;
            private set;
        }

        /// <summary>Returns the total download size in bytes for the current session.</summary>
        /// <returns>Download size in bytes.</returns>
        public static long DataDownloaded
        {
            get;
            private set;
        }

        #endregion


        #region MonoBehaviour methods

        public void OnEnable()
        {
            if (instance == null)
            {
                go = gameObject;

                go.name = Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME;

                instance = this;

                internetAvailable = Application.internetReachability != NetworkReachability.NotReachable;

                if (!Util.Helper.isEditorMode && DontDestroy)
                {
                    DontDestroyOnLoad(transform.root.gameObject);
                }

                networkReachability = lastNetworkReachability = Application.internetReachability;

                if (Util.Config.DEBUG)
                    Debug.LogWarning("Using new instance!");
            }
            else
            {
                if (!Util.Helper.isEditorMode && DontDestroy && instance != this)
                {
                    if (!loggedOnlyOneInstance)
                    {
                        Debug.LogWarning("Only one active instance of '" + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME + "' allowed in all scenes!" + System.Environment.NewLine + "This object will now be destroyed.");

                        loggedOnlyOneInstance = true;
                    }

                    Destroy(gameObject, 0.2f);
                }

                if (Util.Config.DEBUG)
                    Debug.LogWarning("Using old instance!");
            }
        }

        public void Awake()
        {
#if !oc_ignore_setup
            if (CustomCheck != null && !Util.Helper.isEditor && !string.IsNullOrEmpty(CustomCheck.URL) && (CustomCheck.URL.Contains("crosstales.com") || CustomCheck.URL.Contains("207.154.226.218")))
            {
                Debug.LogWarning("'Custom Check' uses 'crosstales.com' for detection: this is only allowed for test-builds and the check interval will be limited!" + System.Environment.NewLine + "Please use your own URL for detection.");
                IntervalMin = Mathf.Clamp(IntervalMin, 30, 119);
                IntervalMax = Mathf.Clamp(IntervalMax, 60, 120);
            }
#endif
        }

        public void Start()
        {
            if ((RunOnStart || Util.Helper.isEditorMode) && !isRunning)
            {
                Invoke("run", Delay);
            }
        }

        public void Update()
        {
#if !oc_ignore_setup
            if (CustomCheck != null && !Util.Helper.isEditor && !string.IsNullOrEmpty(CustomCheck.URL) && (CustomCheck.URL.Contains("crosstales.com") || CustomCheck.URL.Contains("207.154.226.218")))
            {
                IntervalMin = Mathf.Clamp(IntervalMin, 30, 119);
                IntervalMax = Mathf.Clamp(IntervalMax, 60, 120);
            }
#endif
            if (Util.Helper.isEditorMode)
            {
                if (go != null)
                {
                    if (Util.Config.ENSURE_NAME)
                        go.name = Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME; //ensure name
                }
            }
            else
            {
                Util.Context.Runtime += Time.deltaTime;
                if (internetAvailable)
                {
                    Util.Context.Uptime += Time.deltaTime;
                }
            }

            if (EndlessMode && !isRunning)
            {
                checkTimeCounter += Time.deltaTime;
                burstTimeCounter += Time.deltaTime;

                if (isInternetAvailable)
                {
                    if (checkTimeCounter > checkTime)
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Normal-Mode!");

                        checkTimeCounter = 0f;
                        burstTimeCounter = 0f;

                        StartCoroutine(internetCheck());
                    }
                }
                else
                {
                    if (burstTimeCounter > burstTime)
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Burst-Mode!");

                        checkTimeCounter = 0f;
                        burstTimeCounter = 0f;

                        StartCoroutine(internetCheck());
                    }
                }
            }

        }

        public void OnApplicationQuit()
        {
#if !UNITY_WSA || UNITY_EDITOR
            if (worker != null && worker.IsAlive)
            {
                if (Util.Constants.DEV_DEBUG)
                    Debug.Log("Kill worker!");

                worker.Abort();
            }
#endif

            if (instance != null)
            {
                instance.StopAllCoroutines();
            }
        }

        public void OnValidate()
        {
            if (Delay < 0f)
                Delay = 0f;

            if (IntervalMin < 3)
                IntervalMin = 3;

            if (IntervalMin > 119)
                IntervalMin = 119;

            if (IntervalMax < 4)
                IntervalMax = 4;

            if (IntervalMax > 120)
                IntervalMax = 120;

            if (IntervalMin <= Timeout)
                IntervalMin = Timeout + 1;

            if (IntervalMin >= IntervalMax)
                IntervalMax = IntervalMin + 1;

        }

        #endregion


        #region Public methods

        /// <summary>Resets this object.</summary>
        public static void Reset()
        {
            internetAvailable = false;
            networkReachability = NetworkReachability.NotReachable;
            go = null;
            instance = null;
            loggedOnlyOneInstance = false;
            lastCheckTime = long.MinValue;
        }

        /// <summary>Checks for Internet availability.</summary>
        public static void Refresh()
        {
            if (instance != null && !instance.isRunning && lastCheckTime + 10000000 < System.DateTime.Now.Ticks)
            {
                instance.StartCoroutine(instance.internetCheck());
            }
        }

        /// <summary>Checks for Internet availability as an IEnumerator.</summary>
        public static IEnumerator RefreshYield()
        {
            if (instance != null && !instance.isRunning && lastCheckTime + 10000000 < System.DateTime.Now.Ticks)
            {
                yield return instance.internetCheck();
            }
            else
            {
                yield return null;
            }
        }

        #endregion


        #region Private methods

        private void run()
        {
            StartCoroutine(internetCheck());
        }

        private void createCustomCheck()
        {
            Util.Helper.CreateCustomCheck();
        }

        private IEnumerator wwwCheck(string url, string data, bool equals, string type, bool showError = false)
        {
            available = false;

            using (UnityWebRequest www = UnityWebRequest.Get(URLAntiCacheRandomizer(url)))
            {
#if UNITY_2017_1_OR_NEWER
                www.timeout = Timeout;
#endif
#if UNITY_2017_2_OR_NEWER
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();
#else
                yield return www.Send();
#endif

#if UNITY_2017_1_OR_NEWER
                if (!www.isHttpError && !www.isNetworkError)
#else
                if (string.IsNullOrEmpty(www.error))
#endif
                {
                    string result = www.downloadHandler.text;

                    if (Util.Constants.DEV_DEBUG)
                        Debug.LogWarning("Content from " + type + ": " + result);

                    if (equals)
                    {
                        available = !string.IsNullOrEmpty(result) && result.CTEquals(data);
                    }
                    else
                    {
                        available = !string.IsNullOrEmpty(result) && result.CTContains(data);
                    }
                }
                else
                {
                    if (Util.Constants.DEV_DEBUG || showError)
                        Debug.LogError("Error getting content from " + type + ": " + www.error);
                }

                DataDownloaded += (long)www.downloadedBytes;
            }
        }

        /*
        //disabled since UnityWebRequest seems not to return the correct response headers...
        private IEnumerator google204Check(bool showError = false)
        {
            available = false;
            string url = "https://clients3.google.com/generate_204";

            using (UnityWebRequest www = UnityWebRequest.Get(URLAntiCacheRandomizer(url)))
            {
#if UNITY_2017_1_OR_NEWER
                www.timeout = Timeout;
#endif
#if UNITY_2017_2_OR_NEWER
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();
#else
                yield return www.Send();
#endif

#if UNITY_2017_1_OR_NEWER
                if (!www.isHttpError && !www.isNetworkError)
#else
                if (string.IsNullOrEmpty(www.error))
#endif
                {
                    Dictionary<string, string> responseHeaders = www.GetResponseHeaders();

                    string result = www.downloadHandler.text;

                    if (Util.Constants.DEV_DEBUG)
                        Debug.LogWarning("Content from Google204: " + result);

                    if (responseHeaders != null && responseHeaders.Keys != null && responseHeaders.Keys.Count > 0)
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log(responseHeaders.CTDump());

                        string httpStatus = string.Empty;

                        if (responseHeaders.ContainsKey("STATUS")) // default on most platforms
                            httpStatus = responseHeaders["STATUS"];
                        else if (responseHeaders.ContainsKey("NULL")) // Android
                            httpStatus = responseHeaders["NULL"];

                        if (httpStatus.Length > 0)
                        {
                            available = httpStatus.CTContains("204 No Content");
                        }
                    }
                    else
                    {
                        available = www.downloadedBytes == 0;
                    }
                }
                else
                {
                    if (Util.Constants.DEV_DEBUG || showError)
                        Debug.LogError("Error getting content from Google204: " + www.error);
                }

                DataDownloaded += (long)www.downloadedBytes;
            }
        }
        */

        private IEnumerator googleBlankCheck(bool showError = false)
        {
            available = false;
            string url = "https://www.google.com/blank.html";

            using (UnityWebRequest www = UnityWebRequest.Get(URLAntiCacheRandomizer(url)))
            {
#if UNITY_2017_1_OR_NEWER
                www.timeout = Timeout;
#endif
#if UNITY_2017_2_OR_NEWER
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();
#else
                yield return www.Send();
#endif

#if UNITY_2017_1_OR_NEWER
                if (!www.isHttpError && !www.isNetworkError)
#else
                if (string.IsNullOrEmpty(www.error))
#endif
                {
                    string result = www.downloadHandler.text;

                    if (Util.Constants.DEV_DEBUG)
                        Debug.LogWarning("Content from GoogleBlank: " + result);

                    available = www.downloadedBytes == 0;
                }
                else
                {
                    if (Util.Constants.DEV_DEBUG || showError)
                        Debug.LogError("Error getting content from GoogleBlank: " + www.error);
                }

                DataDownloaded += (long)www.downloadedBytes;
            }
        }

        private void threadCheck(out bool available, string url, string data, bool equals, string type, bool showError = false)
        {
            available = false;
            string content = string.Empty;

#if !UNITY_WSA || UNITY_EDITOR
            try
            {
                using (System.Net.WebClient client = new Util.CTWebClientNotCached(Timeout * 1000))
                {
                    content = client.DownloadString(url);

                    if (Util.Constants.DEV_DEBUG)
                        Debug.LogWarning("Content from " + type + ": " + content);

                    if (equals)
                    {
                        available = !string.IsNullOrEmpty(content) && content.CTEquals(data);
                    }
                    else
                    {
                        available = !string.IsNullOrEmpty(content) && content.CTContains(data);
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (Util.Constants.DEV_DEBUG || showError)
                    Debug.LogError("Error getting content from " + type + ": " + ex);
            }
#endif

            DataDownloaded += content.Length;
        }

        private IEnumerator startWorker(string url, string data, bool equals, string type, bool showError = false)
        {
#if !UNITY_WSA || UNITY_EDITOR
            if (worker == null || !worker.IsAlive)
            {
                worker = new System.Threading.Thread(() => threadCheck(out available, url, data, equals, type, showError));
                worker.Start();

                do
                {
                    yield return null;
                } while (worker.IsAlive);
            }
#else
            yield return null;
#endif
        }

        private IEnumerator internetCheck()
        {
            if (!isRunning)
            {
                Util.Context.NumberOfChecks++;

                isRunning = true;

                available = false;

                if (Util.Config.DEBUG)
                    Debug.Log("[" + System.DateTime.Now + "] " + Util.Constants.ASSET_NAME + " running...");

#if OC_UNAVAILABLE
                    Debug.LogWarning("[" + System.DateTime.Now + "] Compile define 'OC_UNAVAILABLE' enabled. Result of the check is always 'UNAVAILABLE'.");

                    available = false;
                    
                    yield return null;
#elif OC_AVAILABLE
                    Debug.LogWarning("[" + System.DateTime.Now + "] Compile define 'OC_AVAILABLE' enabled. Result of the check is always 'AVAILABLE'.");

                    available = true;
                    
                    yield return null;
#else
                if (Util.Helper.isEditorMode)
                {
                    // Unity check
                    if (Util.Constants.DEV_DEBUG)
                        Debug.LogWarning("[" + System.DateTime.Now + "] Editor is using Unity check (= unreliable!).");

                    available = Application.internetReachability != NetworkReachability.NotReachable;

                }
                else if (Util.Helper.isWebPlatform)
                {
                    // Custom check
                    if (CustomCheck != null && !string.IsNullOrEmpty(CustomCheck.URL))
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.LogWarning("[" + System.DateTime.Now + testingDesc + customDesc);

                        yield return wwwCheck(CustomCheck.URL.Trim(), CustomCheck.ExpectedData, CustomCheck.DataMustBeEquals, customDesc, true);

                    }
                    else
                    {
                        // Unity check
                        Debug.LogWarning("CustomCheck not set correctly - using Unity check (= unreliable!).");

                        available = Application.internetReachability != NetworkReachability.NotReachable;
                    }
                }
                else
                {

#if !UNITY_WSA || UNITY_EDITOR
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = Util.Helper.RemoteCertificateValidationCallback;
#endif

                    // Custom check
                    if (CustomCheck != null && !string.IsNullOrEmpty(CustomCheck.URL))
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.LogWarning("[" + System.DateTime.Now + testingDesc + customDesc);

                        if (Util.Helper.isWSAPlatform || ForceWWW)
                        {
                            yield return wwwCheck(CustomCheck.URL.Trim(), CustomCheck.ExpectedData, CustomCheck.DataMustBeEquals, customDesc, CustomCheck.ShowErrors);
                        }
                        else
                        {
                            yield return startWorker(CustomCheck.URL.Trim(), CustomCheck.ExpectedData, CustomCheck.DataMustBeEquals, customDesc, CustomCheck.ShowErrors);
                        }
                    }
                    else
                    {
                        // Unity check
                        if (CustomCheck != null && CustomCheck.UseOnlyCustom)
                        {
                            Debug.LogWarning("CustomCheck not set correctly - using Unity check (= unreliable!).");

                            available = Application.internetReachability != NetworkReachability.NotReachable;
                        }
                    }

                    if (CustomCheck == null || (CustomCheck != null && !CustomCheck.UseOnlyCustom))
                    {
                        // Microsoft check
                        if (!available && !Util.Helper.isAppleBasedPlatform)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.LogWarning("[" + System.DateTime.Now + testingDesc + windowsDesc);

                            if (Util.Helper.isWSAPlatform || ForceWWW)
                            {
                                yield return wwwCheck(microsoftUrl, microsoftText, microsoftEquals, windowsDesc);
                            }
                            else
                            {
                                yield return startWorker(microsoftUrl, microsoftText, microsoftEquals, windowsDesc);
                            }
                        }

                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Microsoft check: " + available);

                        // Apple check
                        if (!available && Util.Helper.isAppleBasedPlatform)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.LogWarning("[" + System.DateTime.Now + testingDesc + appleDesc);

                            startWorker(appleUrl, appleText, appleEquals, appleDesc);
                        }

                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Apple check: " + available);

                        /*
                        // Google204 check
                        if (!available)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.LogWarning("[" + System.DateTime.Now + testingDesc + "Google204");

                            yield return google204Check();
                        }

                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Google204 check: " + available);
                        */

                        // Ubuntu check
                        if (!available)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.Log("[" + System.DateTime.Now + testingDesc + ubuntuDesc);

                            if (Util.Helper.isWSAPlatform || ForceWWW)
                            {
                                yield return wwwCheck(ubuntutUrl, ubuntuText, ubuntuEquals, ubuntuDesc);
                            }
                            else
                            {
                                yield return startWorker(ubuntutUrl, ubuntuText, ubuntuEquals, ubuntuDesc);
                            }
                        }

                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Ubuntu check: " + available);

                        // GoogleBlank check
                        if (!available)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.LogWarning("[" + System.DateTime.Now + testingDesc + "GoogleBlank");

                            yield return googleBlankCheck();
                        }

                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("GoogleBlank check: " + available);

                        // Fallback check
                        if (!available)
                        {
                            if (Util.Constants.DEV_DEBUG)
                                Debug.Log("[" + System.DateTime.Now + testingDesc + fallbackDesc);

                            if (Util.Helper.isWSAPlatform || ForceWWW)
                            {
                                yield return wwwCheck(fallbackUrl, fallbackText, fallbackEquals, fallbackDesc);
                            }
                            else
                            {
                                yield return startWorker(fallbackUrl, fallbackText, fallbackEquals, fallbackDesc);
                            }
                        }

                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Fallback check: " + available);
                    }
                }
#endif

                internetAvailable = available;

                if (!internetAvailable || Application.internetReachability == NetworkReachability.NotReachable)
                {
                    networkReachability = NetworkReachability.NotReachable;
                }
                else
                {
                    networkReachability = Application.internetReachability;
                }

                onInternetCheckComplete(internetAvailable, networkReachability);

                if (isFirsttime || internetAvailable != lastInternetAvailable)
                {
                    lastInternetAvailable = internetAvailable;

                    onInternetStatusChange(internetAvailable);
                }

                if (isFirsttime || networkReachability != lastNetworkReachability)
                {
                    lastNetworkReachability = networkReachability;

                    onNetworkReachabilityChange(networkReachability);
                }

                if (Util.Config.DEBUG)
                {
                    if (internetAvailable) Debug.Log("[" + System.DateTime.Now + "] Internet access AVAILABLE!");
                    else Debug.LogWarning("[" + System.DateTime.Now + "] Internet access UNAVAILABLE!");
                }

                checkTime = Random.Range(CheckIntervalMin, CheckIntervalMax);
                burstTime = Random.Range(burstIntervalMin, burstIntervalMax);
                lastCheckTime = System.DateTime.Now.Ticks;

                isFirsttime = false;
                isRunning = false;
            }
            else
            {
                Debug.LogError(Util.Constants.ASSET_NAME + " already running!");
            }
        }

        private static string URLAntiCacheRandomizer(string url)
        {
            return url + "?p=" + Random.Range(1, 99999999).ToString();
        }

        #endregion


        #region Event-trigger methods

        private void onInternetStatusChange(bool isConnected)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onInternetStatusChange: " + isConnected);

            if (_onOnlineStatusChange != null)
            {
                _onOnlineStatusChange(isConnected);
            }
        }

        private void onNetworkReachabilityChange(NetworkReachability networkReachability)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onNetworkReachabilityChange: " + networkReachability);

            if (_onNetworkReachabilityChange != null)
            {
                _onNetworkReachabilityChange(networkReachability);
            }
        }

        private void onInternetCheckComplete(bool isConnected, NetworkReachability networkReachability)
        {
            LastCheck = System.DateTime.Now;

            if (Util.Config.DEBUG)
                Debug.Log("onInternetCheckComplete: " + isConnected + " - " + networkReachability);

            if (_onOnlineCheckComplete != null)
            {
                _onOnlineCheckComplete(isConnected, networkReachability);
            }
        }

        #endregion

    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)