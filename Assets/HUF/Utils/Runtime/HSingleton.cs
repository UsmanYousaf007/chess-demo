using UnityEngine;

namespace HUF.Utils.Runtime
{
    public class HSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        protected static bool isQuitting;

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public static T Instance
        {
            get
            {
                if ( instance != null )
                    return instance;

                var t = FindObjectsOfType<T>();

                if ( t.Length == 1 )
                {
                    instance = t[0];
                    return instance;
                }

                if ( t.Length > 1 )
                {
                    Debug.LogError( "More than 1 instance of singleton class!" );
                    return t[0];
                }

                switch ( t.Length )
                {
                    case 0 when isQuitting:
                        return null;
                    case 0:
                    {
                        var go = new GameObject( typeof(T).ToString() );
                        instance = go.AddComponent<T>();
                        go.AddComponent<DontDestroyOnLoad>();
                        return instance;
                    }
                    default:
                        return null;
                }
            }
        }
    }
}