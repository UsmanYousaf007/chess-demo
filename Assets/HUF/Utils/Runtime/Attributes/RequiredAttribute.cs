using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime.Attributes {
    public class RequiredAttribute : PropertyAttribute {
        [CanBeNull]
        public readonly Type TargetType;

        public RequiredAttribute( Type desiredType ) {
            TargetType = desiredType;
        }

        public RequiredAttribute() {}

        public string ShortName => TargetType?.Name;
    }
}