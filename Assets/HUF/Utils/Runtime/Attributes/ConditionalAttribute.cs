using UnityEngine;

namespace HUF.Utils.Runtime.Attributes
{
    public class ConditionalAttribute : PropertyAttribute
    {
        public readonly string conditionProperty;

        public ConditionalAttribute(string conditionProperty)
        {
            this.conditionProperty = conditionProperty;
        }
    }
}