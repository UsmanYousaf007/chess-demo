#if UNITY_IOS

using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;
using System;
using UnityEngine;

public class MegacoolPlistManipulator {

	[PostProcessBuildAttribute(1000)]
    public static void OnPostprocessBuild(BuildTarget bt, string path){
        if (bt != BuildTarget.iOS) {
            // This is only to modify the XCode project file and the Info.plist, not needed for other targets
            return;
        }

        // Not using MegacoolConfiguration.Instance since we don't want a new instance if
        // it's absent, better to issue warning
        MegacoolConfiguration configuration = Resources.Load<MegacoolConfiguration>("MegacoolConfiguration");

        if (configuration == null) {
            Debug.LogWarning("No Megacool configuration found, not adding support for universal linking or URL " +
                "schemes");
            return;
        }

        AddMegacoolToXcodeProject(path, configuration.universalLinksIOS, configuration.schemeIOS);
    }

    public static void AddMegacoolToXcodeProject(string path, bool addAssociatedDomains, string customScheme) {

        // Get XCode Project
        string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject project = new PBXProject();
        project.ReadFromFile( projectPath );

        LinkWithDependencies(project);

        string serializedProject = project.WriteToString();

        if (addAssociatedDomains) {
            string entitlementsFilename = CreateMegacoolEntitlements(path);
            serializedProject = AddEntitlementsToSerializedXcodeProject(serializedProject, entitlementsFilename);
        }

        // Save the XCode project
        File.WriteAllText(projectPath , serializedProject);

        // The URL scheme is stored in Info.plist and not in the project, modify that if we have a custom scheme
        if (String.IsNullOrEmpty(customScheme)) {
            Debug.LogWarning ("Megacool: Missing URL Scheme. Please set it in Megacool -> Configuration -> URL Scheme");
        } else {
            AddMegacoolURLScheme(path, customScheme);
        }

    }


    private static void LinkWithDependencies(PBXProject project) {
        string target = project.TargetGuidByName("Unity-iPhone");
        project.AddFrameworkToProject(target, "libz.1.2.5.tbd", false);
    }


    /// <summary>
    /// Either modifies the existing entitlements to add the associated domains capability, or moves the entitlements
    /// bundled with the SDK into the new project and returns that.
    /// </summary>
    private static string CreateMegacoolEntitlements(string path) {
        string entitlementsFilename = "app.entitlements";
        string entitlementsPath = null;

        //Check if the entitlements file is created by another script first
        foreach (string filePath in Directory.GetFiles(path + "/Unity-iPhone/", "*.entitlements")) {
            //Should only be 1 .entitlements file
            entitlementsPath = filePath;
            entitlementsFilename = Path.GetFileName(filePath);
            //Debug.Log("path: "+entitlementsPath + " - name: " + entitlementsFilename);
        }

        if (entitlementsPath != null) {
            AddMegacoolCapabilitiesToEntitlements(entitlementsPath);
        } else {
            // Copy the entitlements file to Unity-iPhone folder
            string srcFilePath = Application.dataPath + "/Megacool/Editor/"+entitlementsFilename;
            string dstFilePath = path + "/Unity-iPhone/" + entitlementsFilename;
            //Debug.LogWarning( string.Format( "Copy app.entitlements from {0} to {1}" , srcFilePath , dstFilePath ) );
            File.Copy( srcFilePath , dstFilePath );
        }
        return entitlementsFilename;
    }


    private static string AddEntitlementsToSerializedXcodeProject(string serializedProject, string entitlementsFilename) {
        // Add entitlements file
        serializedProject = serializedProject.Replace("/* Begin PBXFileReference section */",
            "/* Begin PBXFileReference section */\n\t\t244C317F1B8BE5CF00F39B20 /* "+entitlementsFilename+" */ = {isa = PBXFileReference; lastKnownFileType = text.xml; name = "+entitlementsFilename+"; path = \"Unity-iPhone/"+entitlementsFilename+"\"; sourceTree = \"<group>\"; };");

        // Add entitlements file (again)
        serializedProject = serializedProject.Replace("/* CustomTemplate */ = {\n\t\t\tisa = PBXGroup;\n\t\t\tchildren = (",
            "/* CustomTemplate */ = {\n\t\t\tisa = PBXGroup;\n\t\t\tchildren = (\n\t\t\t\t244C317F1B8BE5CF00F39B20 /* "+entitlementsFilename+" */,");

        // Set code signing for the entitlements file
        serializedProject = serializedProject.Replace("CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS = YES;",
            "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS = YES;\n\t\t\t\tCODE_SIGN_ENTITLEMENTS = \"Unity-iPhone/"+entitlementsFilename+"\";");

        return serializedProject;
    }


    public static void AddMegacoolCapabilitiesToEntitlements(string pathToFile) {

        string associatedDomainsValue = "com.apple.developer.associated-domains";
        string megacoolAppLink = "applinks:mgcl.co";

        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(pathToFile));

        // Get root
        PlistElementDict rootDict = plist.root;
        PlistElementArray associatedDomains = null;

        //Check if com.apple.developer.associated-domains exists in plist
        if (!rootDict.values.ContainsKey(associatedDomainsValue)) {
            associatedDomains = rootDict.CreateArray(associatedDomainsValue);
        } else {
            associatedDomains = rootDict.values[associatedDomainsValue].AsArray();

            if (associatedDomains == null) {
                associatedDomains = rootDict.CreateArray(associatedDomainsValue);
            }
        }

        bool megacoolAppLinkExists = false;
        foreach(PlistElement elem in associatedDomains.values) {
            if (elem.AsString() != null && elem.AsString().Equals(megacoolAppLink)) {
                megacoolAppLinkExists = true;
                break;
            }
        }

        //Add applinks:mgcl.co
        if (!megacoolAppLinkExists) {
            associatedDomains.AddString(megacoolAppLink);
        }

        // Write to file
        File.WriteAllText(pathToFile, plist.WriteToString());

    }

    public static void AddMegacoolURLScheme(string pathToBuildProject, string customScheme) {
        string plistPath = pathToBuildProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));


        // Get root
        PlistElementDict rootDict = plist.root;
        PlistElementArray urlTypesArray = null;
        PlistElementDict  urlTypesItems = null;
        PlistElementArray urlSchemesArray = null;

        if (!rootDict.values.ContainsKey("CFBundleURLTypes")) {
            urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
        } else {
            urlTypesArray = rootDict.values["CFBundleURLTypes"].AsArray();

            if (urlTypesArray == null) {
                urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
            }
        }

        if (urlTypesArray.values.Count == 0) {
            urlTypesItems = urlTypesArray.AddDict();
        } else {
            urlTypesItems = urlTypesArray.values[0].AsDict();

            if (urlTypesItems == null) {
                urlTypesItems = urlTypesArray.AddDict();
            }
        }

        if (!urlTypesItems.values.ContainsKey("CFBundleURLSchemes")) {
            urlSchemesArray = urlTypesItems.CreateArray("CFBundleURLSchemes");
        } else {
            urlSchemesArray = urlTypesItems.values["CFBundleURLSchemes"].AsArray();

            if (urlSchemesArray == null) {
                urlSchemesArray = urlTypesItems.CreateArray("CFBundleURLSchemes");
            }
        }

        bool megacoolURLSchemeExists = false;
        foreach(PlistElement elem in urlSchemesArray.values) {
            if (elem.AsString() != null && elem.AsString().Equals(customScheme)) {
                megacoolURLSchemeExists = true;
                break;
            }
        }

        if (!megacoolURLSchemeExists) {
            urlSchemesArray.AddString(customScheme);
        }

        // Write to file
        File.WriteAllText(plistPath, plist.WriteToString());

    }
}
#endif
