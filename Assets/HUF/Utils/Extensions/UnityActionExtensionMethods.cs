using System;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Utils.Extensions
{
    public static class UnityActionExtensionMethods
    {
        public static void Dispatch(this UnityAction action)
        {
            if (action == null)
                return;
            
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void Dispatch<T0>(this UnityAction<T0> action, T0 parameter)
        {
            if (action == null) 
                return;
            
            try
            {
                action(parameter);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void Dispatch<T0, T1>(this UnityAction<T0, T1> action, T0 parameter1, T1 parameter2)
        {
            if (action == null) 
                return;
            
            try
            {
                action(parameter1, parameter2);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}