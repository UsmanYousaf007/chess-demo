using System;
using HUF.Utils.Attributes;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Utils.Configs.API
{
    public abstract class AbstractConfig : ScriptableObject
    {
        [SerializeField] [ConfigId] protected string configId;

        public string ConfigId => configId;

        public event UnityAction<AbstractConfig> OnChanged;
        public event UnityAction OnChangedInEditor;

        void OnEnable()
        {
            if (string.IsNullOrEmpty(configId))
                configId = GetType().Name;
        }

        public void ApplyJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                JsonUtility.FromJsonOverwrite(json, this);
                Debug.Log($"[Config] Json applied to config: {configId}.");
                
                ValidateConfig();
                OnChanged.Dispatch(this);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Config] Problem with parsing Json for config {GetType().Name} type" +
                               $"\n{e.Message}");
            }
        }

        public virtual void ValidateConfig()
        {
            
        }

        public void ResetOnChanged()
        {
            OnChanged = null;
        }
        
        void OnValidate()
        {
            if (Application.isEditor)
            {
                OnChangedInEditor.Dispatch();
            }
        }
    }
}