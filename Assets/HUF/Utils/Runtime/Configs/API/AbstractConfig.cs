using System;
using System.Text.RegularExpressions;
using HUF.Utils.Runtime.Attributes;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Build;
using HUF.Utils.Runtime.Configs.Implementation;
#endif

namespace HUF.Utils.Runtime.Configs.API
{
    public abstract class AbstractConfig : ScriptableObject
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AbstractConfig) );

        [SerializeField] [ConfigId] protected string configId;

        public string ConfigId => configId;

#pragma warning disable 67
        public event Action<AbstractConfig> OnChanged;
        public event Action OnChangedInEditor;
#pragma warning restore 67

        protected virtual void OnEnable()
        {
            if ( string.IsNullOrEmpty( configId ) )
            {
                configId = GetType().Name;
            }
        }

        public void ApplyJson( string json )
        {
            if ( string.IsNullOrEmpty( json ) )
            {
                return;
            }

            try
            {
                JsonUtility.FromJsonOverwrite( RemoveObjectReferences( json ), this );
                HLog.Log( logPrefix, $"Json applied to config: {configId}." );
                ValidateConfig();
                OnChanged.Dispatch( this );
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, $"Problem with parsing Json for config {GetType().Name} type\n{e.Message}" );
            }
        }

        public static string RemoveObjectReferences( string json )
        {
            if ( !json.Contains( "\"instanceID\"" ) )
            {
                return json;
            }

            json = Regex.Replace( json, ",\"\\w+\":{\"instanceID\":\\w+}", string.Empty );

            if ( !json.Contains( string.Empty ) )
                return json;

            //check for array with objects
            json = Regex.Replace( json, ",{\"instanceID\":\\w+}", string.Empty );
            json = Regex.Replace( json, "{\"instanceID\":\\w+}", string.Empty );
            json = Regex.Replace( json, ",\"\\w+\":" + @"\[\]", string.Empty );
            return json;
        }

        public virtual void ValidateConfig() { }

#if UNITY_EDITOR
        public virtual bool IsEditorConfigValid()
        {
            return true;
        }
#endif

        public void ResetOnChanged()
        {
            OnChanged = null;
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            OnChangedInEditor.Dispatch();
#endif
        }

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            ConfigUtils.OverlayPreset( this );
#endif
        }

#if UNITY_EDITOR
        public void OnPreprocessBuild()
        {
            const string SELFCHECK_FAIL = "Config's self check failed";

            if ( !IsEditorConfigValid() )
            {
                HLog.LogError( new HLogPrefix( logPrefix, configId ), SELFCHECK_FAIL );
                UnityEditor.Selection.activeObject = this;
                throw new BuildFailedException( SELFCHECK_FAIL );
            }

            if ( !ConfigPrevalidator.ValidateConfig( this, out string message ) )
            {
                HLog.LogError( new HLogPrefix( logPrefix, configId ), message );
                UnityEditor.Selection.activeObject = this;
                throw new BuildFailedException( message );
            }
        }

        [ContextMenu( "TestValidation" )]
        public void TestValidation()
        {
            try
            {
                OnPreprocessBuild();
                HLog.Log( new HLogPrefix( logPrefix, configId ), "Config valid" );
            }
            catch ( Exception e )
            {
                HLog.LogError( new HLogPrefix( logPrefix, configId ), $"Config Validation failed with: {e}" );
            }
        }
#endif
    }
}
