using System;
using HUF.Utils.Runtime.Attributes;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using HUF.Utils.Runtime.Configs.Implementation;
#endif

namespace HUF.Utils.Runtime.Configs.API
{
    public abstract class AbstractConfig : ScriptableObject
#if UNITY_EDITOR
        , IPreprocessBuildWithReport
#endif
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(AbstractConfig) );

        [SerializeField] [ConfigId] protected string configId;

        public int callbackOrder => 10;
        public string ConfigId => configId;

        public event UnityAction<AbstractConfig> OnChanged;
        public event UnityAction OnChangedInEditor;

        protected virtual void OnEnable()
        {
            if ( string.IsNullOrEmpty( configId ) )
            {
                configId = GetType().Name;
            }
        }

        public void ApplyJson(string json)
        {
            if ( string.IsNullOrEmpty( json ) )
            {
                return;
            }

            try
            {
                JsonUtility.FromJsonOverwrite( json, this );
                HLog.Log( logPrefix, $"Json applied to config: {configId}." );

                ValidateConfig();
                OnChanged.Dispatch(this);
            }
            catch (Exception e)
            {
                HLog.LogError( logPrefix, $"Problem with parsing Json for config {GetType().Name} type\n{e.Message}" );
            }
        }

        public virtual void ValidateConfig()
        {

        }

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

#if UNITY_EDITOR
        public void OnPreprocessBuild( BuildReport report )
        {
            const string SELFCHECK_FAIL = "Config's self check failed";
            if ( !IsEditorConfigValid() )
            {
                HLog.LogError( new HLogPrefix( logPrefix, configId ), SELFCHECK_FAIL );
                UnityEditor.Selection.activeObject = this;
                throw new BuildFailedException( SELFCHECK_FAIL );
            }

            if ( !ConfigPrevalidator.ValidateConfig( this, out string messge ) )
            {
                HLog.LogError( new HLogPrefix( logPrefix, configId ), messge );
                UnityEditor.Selection.activeObject = this;
                throw new BuildFailedException( messge );
            }
        }

        [ContextMenu( "TestValidation" )]
        public void TestValidation()
        {
            try
            {
                OnPreprocessBuild( null );
                HLog.Log( new HLogPrefix( logPrefix, configId ), "Config valid" );
            }
            catch ( Exception e ) { }
        }
#endif
    }
}