using UnityEngine;

namespace HUF.Utils.Attributes
{
    public class WarningAttribute : PropertyAttribute
    {
        public readonly float size;

        public WarningAttribute()
        {
            size = 0;
        }
        
        public WarningAttribute(float size)
        {
            this.size = size;
        }
    }
}