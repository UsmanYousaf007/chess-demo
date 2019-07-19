using UnityEditor;

namespace Crosstales.OnlineCheck.EditorTask
{
    /// <summary>Adds the given define symbols to PlayerSettings define symbols.</summary>
    [InitializeOnLoad]
    public class CompileDefines : Common.EditorTask.BaseCompileDefines
    {

        private static readonly string[] symbols = new string[] {
            "CT_OC",
        };

        static CompileDefines()
        {
            setCompileDefines(symbols); //TODO replace with addSymbolsToAllTargets
        }
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)