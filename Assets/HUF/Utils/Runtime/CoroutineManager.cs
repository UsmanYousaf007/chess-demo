using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public class CoroutineManager : HSingleton<CoroutineManager>
    {
        /// <summary>
        /// Starts a coroutine.
        /// </summary>
        /// <param name="routine">An enumerator.</param>
        /// <returns>A coroutine</returns>
        [PublicAPI]
        public new static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (isQuitting)
                return null;
            
            return ((MonoBehaviour) Instance).StartCoroutine(routine);
        }

        /// <summary>
        /// Stops a coroutine.
        /// </summary>
        /// <param name="routine">A coroutine.</param>
        [PublicAPI]
        public new static void StopCoroutine(Coroutine routine)
        {
            if (isQuitting)
                return;
            
            ((MonoBehaviour) Instance).StopCoroutine(routine);
        }

        /// <summary>
        /// <para>Stops all coroutines on CoroutineManager.</para>
        /// <para>Coroutines are also stopped when CoroutineManager is destroyed.</para>
        /// </summary>
        [PublicAPI]
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