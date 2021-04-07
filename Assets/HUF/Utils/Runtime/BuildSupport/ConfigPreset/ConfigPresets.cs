#if UNITY_EDITOR
using System;
using System.Linq;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace HUF.Utils.Runtime.BuildSupport.ConfigPreset
{
    enum PresetApplyMode
    {
        Automatic = 0,
        ForceDev = 1,
        ForceProd = 2
    }

    [Serializable]
    public struct ConfigPreset
    {
        public string name;
        public ScriptableObject targetConfig;
        public Preset developmentPreset;
        public Preset productionPreset;
        public RuntimePlatform[] platforms;
    }

    [CreateAssetMenu( fileName = "ConfigPresets", menuName = "HUF/Configs/ConfigPresets" )]
    public class ConfigPresets : ScriptableObject
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(ConfigPresets) );
        [SerializeField] ConfigPreset[] presets = default;
        [SerializeField] PresetApplyMode presetApplyMode = default;

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void EditorApplyConfigPresets()
        {
            ApplyConfigPresets( Debug.isDebugBuild );
        }

        public static void ApplyConfigPresets( bool isDebugBuild )
        {
#if UNITY_ANDROID
            const RuntimePlatform currentPlatform = RuntimePlatform.Android;
#elif UNITY_IOS
            const RuntimePlatform currentPlatform = RuntimePlatform.IPhonePlayer;
#elif UNITY_STANDALONE_OSX
            RuntimePlatform currentPlatform = Debug.isDebugBuild ? RuntimePlatform.OSXEditor: RuntimePlatform.OSXPlayer;
#else
            RuntimePlatform currentPlatform =
 Debug.isDebugBuild ? RuntimePlatform.WindowsEditor: RuntimePlatform.WindowsPlayer;
#endif
            var allConfigPresets = AssetDatabase.FindAssets( $"t:{nameof(ConfigPresets)}" );

            if ( allConfigPresets.Length < 1 )
            {
                HLog.Log( logPrefix, "No ConfigPresets found to apply" );
                return;
            }

            if ( allConfigPresets.Length > 1 )
            {
                HLog.LogWarning( logPrefix, "More then one (1) ConfigPresets found, fix this" );
                return;
            }

            var configPresets = (ConfigPresets)AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath( allConfigPresets[0] ),
                typeof(ConfigPresets) );

            for ( var i = 0; i < configPresets.presets.Length; i++ )
            {
                var preset = configPresets.presets[i];

                if ( !preset.platforms.Contains( currentPlatform ) )
                    continue;

                Preset presetToApply = null;

                switch ( configPresets.presetApplyMode )
                {
                    case PresetApplyMode.Automatic:
                        presetToApply = isDebugBuild ? preset.developmentPreset : preset.productionPreset;
                        break;
                    case PresetApplyMode.ForceDev:
                        presetToApply = preset.developmentPreset;
                        break;
                    case PresetApplyMode.ForceProd:
                        presetToApply = preset.productionPreset;
                        break;
                }

                if ( presetToApply == null || preset.targetConfig == null ||
                     !presetToApply.CanBeAppliedTo( preset.targetConfig ) )
                {
                    HLog.LogWarning( logPrefix, "Preset to apply or target config is null" );
                    continue;
                }

                presetToApply.ApplyTo( preset.targetConfig );
                EditorUtility.SetDirty( preset.targetConfig );
            }

            HLog.Log( logPrefix, "Presets apply" );
        }
    }
}
#endif