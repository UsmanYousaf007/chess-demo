namespace HUF.Utils.SafeArea
{
    public static class ScreenSize
    {
        public static float Width
        {
            get {
#if UNITY_EDITOR
                return UnityEditor.Handles.GetMainGameViewSize().x;
#else
                return UnityEngine.Screen.width;
#endif
            }
        }

        public static float Height
        {
            get {
#if UNITY_EDITOR
                return UnityEditor.Handles.GetMainGameViewSize().y;
#else
                return UnityEngine.Screen.height;
#endif
            }
        }

        public static float AspectRatio =>
            UnityEngine.Mathf.Max( Width, Height ) /
            UnityEngine.Mathf.Min( Width, Height );
    }
}