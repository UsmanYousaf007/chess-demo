using UnityEditor;
using UnityEditor.Build;
using System.IO;
using UnityEditor.Build.Reporting;
//public class PreBuildStep : IPreprocessBuild
public class PreBuildStep : IPreprocessBuildWithReport
{
#if UNITY_EDITOR
    public int callbackOrder { get { return 0; } }
    public string TMProRunTimeSourcePath = "/data/turbo-labz/projects/instant-chess/PreBuildStep/";
    public string TMProRunTimeDestinationPath = "/data/turbo-labz/projects/instant-chess/Library/PackageCache/com.unity.textmeshpro@2.0.1/Scripts/Runtime";
    public string[] TMProRunTimeFileNames = new string[] {"TMPro_Private.cs","TMPro_UGUI_Private.cs"};

    //public void OnPreprocessBuild(BuildTarget target, string path)
    public void OnPreprocessBuild(BuildReport report)
    {
        TurboLabz.TLUtils.LogUtil.Log("PREBUILD STEP", "yellow");

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        TMProRunTimeDestinationPath = desktopPath + TMProRunTimeDestinationPath;
        TMProRunTimeSourcePath = desktopPath + TMProRunTimeSourcePath;

        for (int i = 0; i < TMProRunTimeFileNames.Length; i++)
        {
			if (!Directory.Exists(TMProRunTimeDestinationPath))
			{
				Directory.CreateDirectory(TMProRunTimeDestinationPath);
			}

			string sourceFilePath = Path.Combine(TMProRunTimeSourcePath, TMProRunTimeFileNames[i]);
			string destinationFilePath = Path.Combine(TMProRunTimeDestinationPath, TMProRunTimeFileNames[i]);
			if (File.Exists(destinationFilePath))
			{
				File.Delete(destinationFilePath);
			}
			File.Copy(sourceFilePath, destinationFilePath);

            TurboLabz.TLUtils.LogUtil.Log("Copy file:" + sourceFilePath + " To:" + destinationFilePath, "yellow");

        }
    }
#endif
}
