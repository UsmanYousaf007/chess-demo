using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Text;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;

public class MegacoolAndroidManifestEditor : ScriptableObject {
    static char delim = System.IO.Path.DirectorySeparatorChar;

    private static string ApplicationIdentifier {
        get {
#if UNITY_5_6_OR_NEWER
            return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#else
            return PlayerSettings.bundleIdentifier;
#endif
        }
    }

    public static bool IsDefaultApplicationIdentifer () {
        return ApplicationIdentifier == "" || ApplicationIdentifier == "com.Company.ProductName";
    }

    static string _stringsDotXMLPath = "/Plugins/Android/Megacool/res/values/strings.xml";
    public static FileInfo StringsDotXMLTemplate { 
        get { 
            return new FileInfo(Application.dataPath + delim + "Megacool/Editor/strings.byte"); 
        } 
    }

    public static FileInfo MegacoolAndroidManifestFile { 
        get { 
            return new FileInfo(Application.dataPath + delim + "Plugins/Android/Megacool/AndroidManifest.xml"); 
        } 
    }

    public static void ReplaceMegacoolManifestApplicationId() {
        RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase;

        var prefix = "android:authorities=";

        Regex regexPattern = new Regex(prefix + "\"(.+)\\.megacool_file_provider\"", options);
        string replaceWith = string.Format(prefix + "\"{0}.megacool_file_provider\"", ApplicationIdentifier);

        string m_megacoolAndroidManifestText = ReadAllFileText(MegacoolAndroidManifestFile);

        WriteTextToFile(
            MegacoolAndroidManifestFile,
            regexPattern.Replace(m_megacoolAndroidManifestText, replaceWith)
        );
    }

    public static void WriteStringsDotXML(string AppIdentifier, bool UniversalLinksEnabled, string Scheme) {
        CreateValuesFolder();

        if (File.Exists(Application.dataPath + _stringsDotXMLPath)) {
            File.Delete(Application.dataPath + _stringsDotXMLPath);
        }

        File.Copy(StringsDotXMLTemplate.FullName, Application.dataPath + _stringsDotXMLPath);

        var m_stringsDotXMLReplaced = string.Format(ReadAllFileText(StringsDotXMLTemplate), AppIdentifier, Scheme);
        WriteTextToFile(
            new FileInfo(Application.dataPath + _stringsDotXMLPath), 
            m_stringsDotXMLReplaced
        );
    }

    #region Util

    public static void WriteTextToFile(FileInfo file, string text) {
        using (StreamWriter m_writer = new StreamWriter(file.FullName)) {
            m_writer.Write(text);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static string ReadAllFileText(FileInfo file) {
        var m_returnText = string.Empty;
        using (StreamReader sr = new StreamReader(file.FullName)) {
            string line;
            while ((line = sr.ReadLine()) != null) {
                m_returnText += line + '\n';
            }
        }
        return m_returnText;
    }

    private static void TryCopyFile(FileInfo fromFile, FileInfo toFile) {
        if (!toFile.Exists) {
            FileUtil.CopyFileOrDirectory(fromFile.FullName, toFile.FullName);
        }
    }

    public static void CreateValuesFolder() {
        if (!AssetDatabase.IsValidFolder("Assets/Plugins/Android/Megacool/Res/values")) {
            AssetDatabase.CreateFolder("Assets/Plugins/Android/Megacool/Res", "values");
        }
    }

    #endregion
}
