#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
using UnityEditor.iOS.Xcode;

public class BuildPostProcessor : MonoBehaviour {
    
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path) {
        
        if (buildTarget == BuildTarget.iOS) {
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            
            PlistElementDict rootDict = plist.root;
            
            if (rootDict["NSAppTransportSecurity"] == null) {
                rootDict.CreateDict("NSAppTransportSecurity");
            }
            
            rootDict["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
            rootDict["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoadsInWebContent", true);
            
            var exceptionDomains = rootDict["NSAppTransportSecurity"].AsDict().CreateDict("NSExceptionDomains");
            var domain = exceptionDomains.CreateDict(Debug.isDebugBuild ? "hbi-ingest-dev.net" : "hbi-ingest.net");
            
            domain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
            domain.SetBoolean("NSIncludesSubdomains", true);
            
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
    
}
#endif
