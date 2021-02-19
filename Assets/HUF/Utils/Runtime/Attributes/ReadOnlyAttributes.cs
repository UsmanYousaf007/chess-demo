using UnityEngine;

namespace HUF.Utils.Runtime.Attributes
{
    /// <summary>
    /// Displays a field as read-only in the inspector.
    /// CustomPropertyDrawers will not work when this attribute is used.
    /// </summary>
    /// <seealso cref="BeginReadOnlyGroupAttribute"/>
    /// <seealso cref="EndReadOnlyGroupAttribute"/>
    public class ReadOnlyAttribute : PropertyAttribute { }

    /// <summary>
    /// Displays one or more fields as read-only in the inspector.
    /// Use <see cref="EndReadOnlyGroupAttribute"/> to close the group.
    /// Works with CustomPropertyDrawers.
    /// </summary>
    /// <seealso cref="EndReadOnlyGroupAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    public class BeginReadOnlyGroupAttribute : PropertyAttribute { }

    /// <summary>
    /// Closes the read-only group and resume editable fields.
    /// Use with <see cref="BeginReadOnlyGroupAttribute"/>.
    /// </summary>
    /// <seealso cref="BeginReadOnlyGroupAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    public class EndReadOnlyGroupAttribute : PropertyAttribute { }
}