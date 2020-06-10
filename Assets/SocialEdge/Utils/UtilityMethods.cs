using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SocialEdge.Utils
{
    public static class UtilityMethods
    {
        public static bool Empty<T>(this ICollection<T> collection)
        {
            if (collection != null && collection.Count > 0)
                return false;
            else
                return true;
        }
    }
}