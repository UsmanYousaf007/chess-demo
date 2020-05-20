using System.Collections;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public class CoroutineManager : HSingleton<CoroutineManager>
    {
        public new static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (isQuitting)
                return null;
            
            return ((MonoBehaviour) Instance).StartCoroutine(routine);
        }

        public new static void StopCoroutine(Coroutine routine)
        {
            if (isQuitting)
                return;
            
            ((MonoBehaviour) Instance).StopCoroutine(routine);
        }

        public new static void StopAllCoroutines()
        {
            ((MonoBehaviour) Instance).StopAllCoroutines();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}