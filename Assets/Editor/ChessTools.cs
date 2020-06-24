/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TurboLabz.TLUtils;

public class ChessTools : EditorWindow
{
    static string[] gameScenFiles = new string[] {
            "Assets/InstantFramework/Scenes/Splash.unity",
            "Assets/Game/Scenes/Game.unity"
            };

    [MenuItem("TLTools/Load Scene Splash")]
    public static void SwitchToSplashScene()
    {
        LogUtil.Log("Switch to Splash scene", "yellow");
        EditorSceneManager.OpenScene(gameScenFiles[0]);
    }

    [MenuItem("TLTools/Load Scene Game")]
    public static void SwitchToGameScene()
    {
        LogUtil.Log("Switch to Game scene", "yellow");
        EditorSceneManager.OpenScene(gameScenFiles[1]);
    }
}