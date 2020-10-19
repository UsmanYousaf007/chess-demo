#if UNITY_2019_3_OR_NEWER
using System.IO;
using HUF.Utils.Editor.BuildSupport.AssetsBuilder;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HUF.Utils.Editor.BuildSupport
{
    [UsedImplicitly]
    public class Unity2019BuildFixer : IPreprocessBuildWithReport
    {
        const string ANDROID_PLUGINS_FOLDER = HUFBuildAssetsResolver.PLUGIN_FOLDER + "Android";

        const string GRADLE_TEMPLATE_FILENAME = "gradleTemplate.properties";

        const string GRADLE_TEMPLATE_CONTENT = "org.gradle.jvmargs=-Xmx**JVM_HEAP_SIZE**M\n" +
                                               "org.gradle.parallel=true\n" +
                                               "android.useAndroidX=true\n" +
                                               "android.enableJetifier=true\n" +
                                               "**ADDITIONAL_PROPERTIES**";

        const string LAUNCHER_TEMPLATE_FILENAME = "launcherTemplate.gradle";

        const string LAUNCHER_TEMPLATE_CONTENT = "apply plugin: 'com.android.application'\n" +
                                                 "\n" +
                                                 "([rootProject] + (rootProject.subprojects as List)).each {\n" +
                                                 "    ext {\n" +
                                                 "        it.setProperty(\"android.useAndroidX\", true)\n" +
                                                 "        it.setProperty(\"android.enableJetifier\", true)\n" +
                                                 "    }\n" +
                                                 "}\n" +
                                                 "\n" +
                                                 "dependencies {\n" +
                                                 "    implementation project(':unityLibrary')\n" +
                                                 "    implementation 'androidx.multidex:multidex:2.0.1'\n" +
                                                 "}\n" +
                                                 "\n" +
                                                 "android {\n" +
                                                 "    compileSdkVersion **APIVERSION**\n" +
                                                 "    buildToolsVersion '**BUILDTOOLS**'\n" +
                                                 "\n" +
                                                 "    compileOptions {\n" +
                                                 "        sourceCompatibility JavaVersion.VERSION_1_8\n" +
                                                 "        targetCompatibility JavaVersion.VERSION_1_8\n" +
                                                 "    }\n" +
                                                 "\n" +
                                                 "    defaultConfig {\n" +
                                                 "        minSdkVersion **MINSDKVERSION**\n" +
                                                 "        targetSdkVersion **TARGETSDKVERSION**\n" +
                                                 "        applicationId '**APPLICATIONID**'\n" +
                                                 "        ndk {\n" +
                                                 "            abiFilters **ABIFILTERS**\n" +
                                                 "        }\n" +
                                                 "        versionCode **VERSIONCODE**\n" +
                                                 "        versionName '**VERSIONNAME**'\n" +
                                                 "        multiDexEnabled true\n" +
                                                 "    }\n" +
                                                 "        \n" +
                                                 "    aaptOptions {\n" +
                                                 "        noCompress = ['.unity3d', '.ress', '.resource', '.obb'**STREAMING_ASSETS**]\n" +
                                                 "        ignoreAssetsPattern = \"!.svn:!.git:!.ds_store:!*.scc:.*:!CVS:!thumbs.db:!picasa.ini:!*~\"\n" +
                                                 "    }**SIGN**\n" +
                                                 "        \n" +
                                                 "        lintOptions {\n" +
                                                 "        abortOnError false\n" +
                                                 "    }\n" +
                                                 "\n" +
                                                 "    buildTypes {\n" +
                                                 "        debug {\n" +
                                                 "            minifyEnabled **MINIFY_DEBUG**\n" +
                                                 "            useProguard **PROGUARD_DEBUG**\n" +
                                                 "            proguardFiles getDefaultProguardFile('proguard-android.txt')**SIGNCONFIG**\n" +
                                                 "            jniDebuggable true\n" +
                                                 "        }\n" +
                                                 "        release {\n" +
                                                 "            minifyEnabled **MINIFY_RELEASE**\n" +
                                                 "            useProguard **PROGUARD_RELEASE**\n" +
                                                 "            proguardFiles getDefaultProguardFile('proguard-android.txt')**SIGNCONFIG**\n" +
                                                 "        }\n" +
                                                 "    }**PACKAGING_OPTIONS****SPLITS**\n" +
                                                 "        **BUILT_APK_LOCATION**\n" +
                                                 "        bundle {\n" +
                                                 "        language {\n" +
                                                 "            enableSplit = false\n" +
                                                 "        }\n" +
                                                 "        density {\n" +
                                                 "            enableSplit = false\n" +
                                                 "        }\n" +
                                                 "        abi {\n" +
                                                 "            enableSplit = true\n" +
                                                 "        }\n" +
                                                 "    }\n" +
                                                 "}**SPLITS_VERSION_CODE****LAUNCHER_SOURCE_BUILD_SETUP**\n";



        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(Unity2019BuildFixer) );
        
        public int callbackOrder => 0;

        public void OnPreprocessBuild( BuildReport report )
        {
#if !HUF_ADS_UNITY
            UnityEditor.PackageManager.Client.Remove( "com.unity.ads" );
#endif
        }

        [MenuItem( "HUF/Unity/2019.3+/FixMyProject" )]
        public static void FixUnity2019_4_LTS()
        {
            CreateTemplate(ANDROID_PLUGINS_FOLDER, GRADLE_TEMPLATE_FILENAME, GRADLE_TEMPLATE_CONTENT);
            CreateTemplate(ANDROID_PLUGINS_FOLDER, LAUNCHER_TEMPLATE_FILENAME, LAUNCHER_TEMPLATE_CONTENT);

            HLog.Log( logPrefix, $"Fixing finished." );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
        }

        static void CreateTemplate(string folder, string filename, string content)
        {
            var filePath = Path.Combine( folder, filename );

            if ( File.Exists( filePath ) )
            {
                HLog.LogWarning( logPrefix, $"File {filePath} already exists, skipping" );
                return;
            }

            using ( StreamWriter writer = File.CreateText( filePath ) )
            {
                writer.Write( content );
                writer.Close();
            }
        }
    }
}

#endif