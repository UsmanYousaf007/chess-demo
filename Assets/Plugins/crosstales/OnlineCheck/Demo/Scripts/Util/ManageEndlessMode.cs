using UnityEngine;

namespace Crosstales.OnlineCheck.Demo.Util
{
    /// <summary>Enable or disable EndlessMode at startup.</summary>
    [HelpURL("https://crosstales.com/media/data/assets/OnlineCheck/api/class_crosstales_1_1_online_check_1_1_demo_1_1_util_1_1_manage_endless_mode.html")]
    public class ManageEndlessMode : MonoBehaviour
    {

        public bool EndlessMode = false;

        public void Awake()
        {
            OnlineCheck.isEndlessMode = EndlessMode;
        }
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)