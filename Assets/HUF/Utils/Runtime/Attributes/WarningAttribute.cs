using System;
using UnityEngine;

namespace HUF.Utils.Runtime.Attributes
{
    public class WarningAttribute : PropertyAttribute
    {
        public WarningAttribute(){}

        [Obsolete("Use parameterless constructor")]
        public WarningAttribute(float unused)
        {
        }
    }
}
