using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Crosstales.OnlineCheck.Demo
{
    /// <summary>Main GUI component for all demo scenes.</summary>
    [HelpURL("https://crosstales.com/media/data/assets/OnlineCheck/api/class_crosstales_1_1_online_check_1_1_demo_1_1_g_u_i_main.html")]
    public class GUIMain : MonoBehaviour
    {

        #region Variables

        // Stuff displayed on the GUI
        public Text Name;
        public Text Version;
        public Text Scene;
        public Image Status;
        public Slider MinimumSlider;
        public Slider MaximumSlider;
        public Text MinValue;
        public Text MaxValue;

        [Header("Stats")]
        public Text Available;
        public Text Reachability;
        public Text LastCheck;
        public Text Total;
        public Text PerMinute;
        public Text Data;
        public Text Runtime;
        public Text Uptime;
        public Text Downtime;

        private Color32 green = new Color32(129, 199, 132, 224);
        private Color32 red = new Color32(150, 39, 39, 224);

        private float elapsedTime = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (Name != null)
            {
                Name.text = Crosstales.OnlineCheck.Util.Constants.ASSET_NAME;
            }

            if (Version != null)
            {
                Version.text = Crosstales.OnlineCheck.Util.Constants.ASSET_VERSION;
            }

            if (Scene != null)
            {
                Scene.text = SceneManager.GetActiveScene().name;
            }

            // Because we don't need Sliders in manual mode, just find them when running in Endless Mode
            if (OnlineCheck.isEndlessMode && MinimumSlider != null && MaximumSlider != null && MinValue != null && MaxValue != null)
            {
                MinimumSlider.value = OnlineCheck.CheckIntervalMin;
                MaximumSlider.value = OnlineCheck.CheckIntervalMax;

                MinValue.text = OnlineCheck.CheckIntervalMin.ToString();
                MaxValue.text = OnlineCheck.CheckIntervalMax.ToString();

                MinimumSlider.onValueChanged.AddListener(delegate { ChangeIntervalMin(); });
                MaximumSlider.onValueChanged.AddListener(delegate { ChangeIntervalMax(); });
            }
        }

        public void Update()
        {
            if (MinimumSlider != null && MaximumSlider != null)
            {
                if (MinimumSlider.value >= MaximumSlider.value)
                {
                    MinimumSlider.value = MaximumSlider.value - 1;
                }
            }

            elapsedTime += Time.deltaTime;

            if (elapsedTime > 1f)
            {
                if (Available != null)
                {
                    Available.text = OnlineCheck.isInternetAvailable ? "yes" : "no";
                }

                if (Reachability != null)
                {
                    Reachability.text = OnlineCheck.NetworkReachability.ToString();
                }

                if (LastCheck != null)
                {
                    LastCheck.text = OnlineCheck.LastCheck.ToString();
                }

                if (Total != null)
                {
                    Total.text = Crosstales.OnlineCheck.Util.Context.NumberOfChecks.ToString();
                }

                if (PerMinute != null)
                {
                    PerMinute.text = Crosstales.OnlineCheck.Util.Context.ChecksPerMinute.ToString("#0.0");
                }

                if (Data != null)
                {
                    Data.text = Common.Util.BaseHelper.FormatBytesToHRF(OnlineCheck.DataDownloaded).ToString();
                }

                if (Runtime != null)
                {
                    Runtime.text = Common.Util.BaseHelper.FormatSecondsToHourMinSec(Crosstales.OnlineCheck.Util.Context.Runtime);
                }

                if (Uptime != null)
                {
                    Uptime.text = Common.Util.BaseHelper.FormatSecondsToHourMinSec(Crosstales.OnlineCheck.Util.Context.Uptime);
                }

                if (Downtime != null)
                {
                    Downtime.text = Common.Util.BaseHelper.FormatSecondsToHourMinSec(Crosstales.OnlineCheck.Util.Context.Downtime);
                }

                elapsedTime = 0f;
            }
        }


        public void OnEnable()
        {
            OnlineCheck.OnOnlineCheckComplete += colorKnob;
        }

        public void OnDisable()
        {
            OnlineCheck.OnOnlineCheckComplete -= colorKnob;
        }

        #endregion


        #region Public methods

        public void Check()
        {
            // Check for Internet availability
            OnlineCheck.Refresh();
        }

        public void ChangeIntervalMin()
        {
            OnlineCheck.CheckIntervalMax = (int)MaximumSlider.value;

            MaxValue.text = OnlineCheck.CheckIntervalMax.ToString();

            OnlineCheck.CheckIntervalMin = (int)MinimumSlider.value;

            MinValue.text = OnlineCheck.CheckIntervalMin.ToString();
        }

        public void ChangeIntervalMax()
        {
            OnlineCheck.CheckIntervalMin = (int)MinimumSlider.value;

            MinValue.text = OnlineCheck.CheckIntervalMin.ToString();

            OnlineCheck.CheckIntervalMax = (int)MaximumSlider.value;

            MaxValue.text = OnlineCheck.CheckIntervalMax.ToString();
        }

        #endregion


        #region Private methods

        private void colorKnob(bool available, NetworkReachability networkReachability)
        {
            if (Status)
            {
                if (available)
                {
                    Status.color = green;
                }
                else
                {
                    Status.color = red;
                }
            }

            //Debug.Log(networkReachability);
        }

        #endregion
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)