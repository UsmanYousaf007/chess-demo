using System.Collections.Generic;
using System.Linq;
using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HUF.PolicyGuard.Editor
{
    public sealed class PolicyTextsValidator : IPreprocessBuildWithReport
    {
        const string CONFIGS_PATH = "Assets/HUF/PolicyGuard/Configs/";
        const string CONFIG_ADS_NAME = "PersonalizedAds/PersonalizedAdsPopupConfig";
        const string CONFIG_GDPR_WITH_ADS_NAME = "GDPR/GDPRAdsPopupConfig";
        const string CONFIG_GDPR_NAME = "GDPR/GDPRPopupConfig";
        const string CONFIG_ATT_NAME = "ATTPreTracking/PreTrackingPopupConfig";
        const string CONFIG_CHANGE_ON_PURPOSE = "HUFConfigChangedOnPurpose";

        const string MESSAGE_CONTENT =
            "The texts in the Policy Guard package are changed, this can end up with not translated text.\n" +
            "It is Your responsibility to update/change them according to legal guidelines";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(PolicyTextsValidator) );
        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild( BuildReport report )
        {
            if ( !HPlayerPrefs.GetBool( CONFIG_CHANGE_ON_PURPOSE ) )
                Execute();
        }

        [MenuItem( "HUF/Configs/Validate Policy Tests" )]
        static void MenuItem()
        {
            Execute();
        }

        static void Execute()
        {
            var configsArray = AssetDatabase.FindAssets( $"t: {nameof(PolicyGuardConfig)}" );

            if ( configsArray.Length == 0 )
            {
                throw new BuildFailedException( $"Missing {nameof(PolicyGuardConfig)} Config" );
            }

            var policyGuardConfig = GetConfig( configsArray );

            if ( CheckTextChanged( policyGuardConfig ) )
            {
                ShowPopup( policyGuardConfig );
            }
        }

        static bool CheckTextChanged( PolicyGuardConfig policyGuardConfig )
        {
            return IsConfigChanged( policyGuardConfig.ReferenceToPersonalizedAdsPopup, CONFIG_ADS_NAME ) ||
                   IsConfigChanged( policyGuardConfig.ReferenceToGDPRPopup, CONFIG_GDPR_NAME ) ||
                   IsConfigChanged( policyGuardConfig.ReferenceToATTPreOptInPopup, CONFIG_ATT_NAME ) ||
                   IsConfigChanged( policyGuardConfig.ReferenceToGDPRWithAdsPopup, CONFIG_GDPR_WITH_ADS_NAME );
        }

        static bool IsConfigChanged( HGenericDialogConfig currentConfig, string configPath )
        {
            if ( currentConfig == null )
                return true;

            if ( AssetDatabase.GetAssetPath( currentConfig )
                .Contains( CONFIGS_PATH + configPath ) )
                return false;

            var origConfig =
                AssetDatabase.LoadAssetAtPath<HGenericDialogConfig>(
                    CONFIGS_PATH + configPath + ".asset" );

            return origConfig.headerTranslation != currentConfig.headerTranslation ||
                   origConfig.contentTranslation != currentConfig.contentTranslation ||
                   origConfig.primaryButtonTranslation != currentConfig.primaryButtonTranslation ||
                   origConfig.secondaryButtonTranslation != currentConfig.secondaryButtonTranslation ||
                   origConfig.tertiaryButtonTranslation != currentConfig.tertiaryButtonTranslation;
        }

        static PolicyGuardConfig GetConfig( IEnumerable<string> configsArray )
        {
            string path = AssetDatabase.GUIDToAssetPath( configsArray.FirstOrDefault() );
            return AssetDatabase.LoadAssetAtPath<PolicyGuardConfig>( path );
        }

        static void ShowPopup( PolicyGuardConfig policyGuardConfig )
        {
            HLog.LogWarning( logPrefix, MESSAGE_CONTENT );

            if ( EditorUtility.DisplayDialog( "IMPORTANT",
                MESSAGE_CONTENT,
                "Revert my changes",
                "I know, it is on purpose") )
            {
                RevertConfig( policyGuardConfig.ReferenceToPersonalizedAdsPopup, CONFIG_ADS_NAME );
                RevertConfig( policyGuardConfig.ReferenceToGDPRPopup, CONFIG_GDPR_NAME );
                RevertConfig( policyGuardConfig.ReferenceToATTPreOptInPopup, CONFIG_ATT_NAME );
                RevertConfig( policyGuardConfig.ReferenceToGDPRWithAdsPopup, CONFIG_GDPR_WITH_ADS_NAME );
                HPlayerPrefs.DeleteKey( CONFIG_CHANGE_ON_PURPOSE );
            }
            else
            {
                HPlayerPrefs.SetBool( CONFIG_CHANGE_ON_PURPOSE, true );
            }
        }

        static void RevertConfig( HGenericDialogConfig currentConfig, string configPath )
        {
            if ( AssetDatabase.GetAssetPath( currentConfig )
                .Contains( CONFIGS_PATH + configPath ) )
                return;

            var origConfig =
                AssetDatabase.LoadAssetAtPath<HGenericDialogConfig>( CONFIGS_PATH + configPath + ".asset" );
            currentConfig.headerTranslation = origConfig.headerTranslation;
            currentConfig.contentTranslation = origConfig.contentTranslation;
            currentConfig.primaryButtonTranslation = origConfig.primaryButtonTranslation;
            currentConfig.secondaryButtonTranslation = origConfig.secondaryButtonTranslation;
            currentConfig.tertiaryButtonTranslation = origConfig.tertiaryButtonTranslation;
        }
    }
}
