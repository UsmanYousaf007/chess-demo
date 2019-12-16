#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.OnlineCheck.EditorTask
{
    /// <summary>Adds the given define symbols to PlayerSettings define symbols.</summary>
    [InitializeOnLoad]
    public class CompileDefines : Common.EditorTask.BaseCompileDefines
    {
        private const string symbol = "CT_OC";

        static CompileDefines()
        {
            addSymbolsToAllTargets(symbol);
        }
    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)