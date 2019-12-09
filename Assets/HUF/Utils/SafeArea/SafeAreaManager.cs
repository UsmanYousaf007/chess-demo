using HUF.Utils.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Utils.SafeArea
{
#if UNITY_EDITOR
    public struct SafeAreaSimulatorHelper
    {
        public const string EDITOR_SAVE_VALUES_KEY = "EditorSafeAreaSimulator";
        public int top;
        public int bottom;
        public int left;
        public int right;
    }
#endif

    
    public class SafeAreaManager : HSingleton<SafeAreaManager>
    {
        public static event UnityAction OnSafeAreaChange;
        Rect cachedSafeArea;
        ScreenOrientation lastScreenOrientation;
        
        public Rect SafeArea {
            get
            {
                if (cachedSafeArea == Rect.zero)
                {
                    CalculateSafeArea();
                }
                return cachedSafeArea;
            }
        }

        void Awake()
        {
            lastScreenOrientation = Screen.orientation;
            CalculateSafeArea();
        }

        void Update()
        {
            if (lastScreenOrientation != Screen.orientation)
            {
                lastScreenOrientation = Screen.orientation;
                CalculateSafeArea();
            }
        }

        void CalculateSafeArea()
        {
            cachedSafeArea = GetSafeArea();
            OnSafeAreaChange.Dispatch();
        }
#if UNITY_EDITOR
        Rect GetSafeArea()
        {
            if (!EditorPrefs.HasKey(SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY))
                return Screen.safeArea;

            var editorArea = JsonUtility.FromJson<SafeAreaSimulatorHelper>(EditorPrefs.GetString(SafeAreaSimulatorHelper.EDITOR_SAVE_VALUES_KEY));

            var editorSafeArea = new Rect(
                editorArea.left,
                editorArea.bottom,
                Screen.width - editorArea.left - editorArea.right,
                UnityEngine.Screen.height - editorArea.top - editorArea.bottom);

            return editorSafeArea;
        }
#else
        Rect GetSafeArea()
        {
            return Screen.safeArea;
        }
#endif
    }
}