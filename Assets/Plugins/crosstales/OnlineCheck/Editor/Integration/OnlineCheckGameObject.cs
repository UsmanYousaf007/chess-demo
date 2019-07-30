#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.OnlineCheck.EditorUtil;

namespace Crosstales.OnlineCheck.EditorIntegration
{
    /// <summary>Editor component for the "Hierarchy"-menu.</summary>
    public class OnlineCheckGameObject : MonoBehaviour
    {
        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME, false, EditorHelper.GO_ID)]
        private static void AddOnlineCheck()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME);
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.ONLINECHECK_SCENE_OBJECT_NAME, true)]
        private static bool AddOnlineCheckValidator()
        {
            return !EditorHelper.isOnlineCheckInScene;
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.PROXY_SCENE_OBJECT_NAME, false, EditorHelper.GO_ID + 2)]
        private static void AddProxy()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.PROXY_SCENE_OBJECT_NAME);
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.PROXY_SCENE_OBJECT_NAME, true)]
        private static bool AddProxyValidator()
        {
            return !EditorHelper.isProxyInScene;
        }
    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)