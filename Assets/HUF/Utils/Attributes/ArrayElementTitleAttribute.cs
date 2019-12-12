using UnityEngine;

namespace HUF.Utils.Attributes
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public readonly string varname;

        public ArrayElementTitleAttribute(string elementTitleVar)
        {
            varname = elementTitleVar;
        }
    }
}