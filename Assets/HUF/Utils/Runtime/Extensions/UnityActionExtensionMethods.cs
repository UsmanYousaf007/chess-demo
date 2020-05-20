using System;
using HUF.Utils.Runtime.Logging;
using UnityEngine.Events;

namespace HUF.Utils.Runtime.Extensions
{
    public static class UnityActionExtensionMethods
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( "Invocation" );

        public static void Dispatch( this UnityAction action )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke();
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke();
#endif
        }

        public static void Dispatch<T0>( this UnityAction<T0> action, T0 parameter )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke( parameter );
#endif
        }

        public static void Dispatch<T0, T1>( this UnityAction<T0, T1> action, T0 parameter1, T1 parameter2 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter1, parameter2 );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke( parameter1, parameter2 );
#endif
        }
        
        public static void Dispatch<T0, T1, T2>( this UnityAction<T0, T1, T2> action, T0 parameter1, T1 parameter2, T2 parameter3 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter1, parameter2, parameter3 );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke( parameter1, parameter2, parameter3 );
#endif
        }
        
        public static void Dispatch<T0, T1, T2, T3>( this UnityAction<T0, T1, T2, T3> action, T0 parameter1, T1 parameter2, T2 parameter3, T3 parameter4 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter1, parameter2, parameter3, parameter4 );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke( parameter1, parameter2, parameter3, parameter4 );
#endif
        }

        public static void Dispatch( this Action action )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke();
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke();
#endif
        }

        public static void Dispatch<T0>( this Action<T0> action, T0 parameter )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke( parameter );
#endif
        }

        public static void Dispatch<T0, T1>( this Action<T0, T1> action, T0 parameter1, T1 parameter2 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter1, parameter2 );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
            action?.Invoke( parameter1, parameter2 );
#endif
        }
        
        public static void Dispatch<T0, T1, T2>( this Action<T0, T1, T2> action, T0 parameter1, T1 parameter2 , T2 parameter3)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter1, parameter2, parameter3 );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
        action?.Invoke( parameter1, parameter2, parameter3 );
#endif
        }
        
        
        public static void Dispatch<T0, T1, T2, T3>( this Action<T0, T1, T2, T3> action, T0 parameter1, T1 parameter2, T2 parameter3, T3 parameter4)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                action?.Invoke( parameter1, parameter2, parameter3, parameter4 );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
            }
#else
        action?.Invoke( parameter1, parameter2, parameter3, parameter4 );
#endif
        }

        public static TR Dispatch<TR>( this Func<TR> function )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                return function != null ? function.Invoke() : default;
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
                return default;
            }
#else
            return function != null ? function.Invoke() : default;
#endif
        }

        public static TR Dispatch<TR, T0>( this Func<T0, TR> function, T0 parameter1 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                return function != null ? function.Invoke( parameter1 ) : default;
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
                return default;
            }
#else
            return function != null ? function.Invoke( parameter1 ) : default;
#endif
        }

        public static TR Dispatch<TR, T0, T1>( this Func<T0, T1, TR> function, T0 parameter1, T1 parameter2 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                return function != null ? function.Invoke( parameter1, parameter2 ) : default;
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
                return default;
            }
#else
            return function != null ? function.Invoke( parameter1 ,parameter2 ) : default;
#endif
        }

        public static TR Dispatch<TR, T0, T1, T2>( this Func<T0, T1, T2, TR> function, T0 parameter1, T1 parameter2, T2 parameter3 )
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                return function != null ? function.Invoke( parameter1, parameter2, parameter3 ) : default;
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"{e.Message} in: {e.StackTrace}" );
                return default;
            }
#else
            return function != null ? function.Invoke( parameter1 ,parameter2, parameter3  ) : default;
#endif
        }
        
    }
}