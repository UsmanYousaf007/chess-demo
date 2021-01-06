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

public partial class ChessTools : EditorWindow
{
    static string[] gameSceneFiles = new string[] {
            "Assets/InstantFramework/Scenes/Splash.unity",
            "Assets/Game/Scenes/Game.unity"
            };
    static int currSceneIndex = -1;

    public static void Validate()
    {
        if (currSceneIndex == -1)
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.name != null)
            {
                if (activeScene.path == gameSceneFiles[0])
                {
                    currSceneIndex = 0;
                }
                else if (activeScene.path == gameSceneFiles[1])
                {
                    currSceneIndex = 1;
                }
            }
        }
    }

    [MenuItem("TLTools/Load Scene Splash", false, 1)]
    public static void SwitchToSplashScene()
    {
        LogUtil.Log("Switch to Splash scene", "yellow");
        SwitchScene(0);
    }

    [MenuItem("TLTools/Load Scene Game", false, 2)]
    public static void SwitchToGameScene()
    {
        LogUtil.Log("Switch to Game scene", "yellow");
        SwitchScene(1);
    }

    
    [MenuItem("TLTools/Show GS Config Environment", false, 25)]
    public static void ShowGSConfigEnvironment()
    {
        GameSparksConfig.Environment env = GetGameSparksEnvironment();
        LogUtil.Log("GameSparks Config: " + env.ToString(), "yellow");
    }


    [MenuItem("TLTools/GS Config Development", false, 26)]
    public static void SetGamesparksEnvDevelopment()
    {
        LogUtil.Log("Switch to GS Config Development", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.Development);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }


    [MenuItem("TLTools/GS Config Live Preview", false, 28)]
    public static void SetGamesparksEnvLivePreview()
    {
        LogUtil.Log("Switch to GS Config Live Preview", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.LivePreview);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    [MenuItem("TLTools/GS Config Live", false, 29)]
    public static void SetGamesparksEnvLive()
    {
        LogUtil.Log("Switch to GS Config Live", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.Live);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    [MenuItem("TLTools/GS Config Sami", false, 40)]
    public static void SetGamesparksEnvSami()
    {
        LogUtil.Log("Switch to GS Config Sami", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.Sami);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    [MenuItem("TLTools/GS Config Omer", false, 41)]
    public static void SetGamesparksEnvOmer()
    {
        LogUtil.Log("Switch to GS Config Omer", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.Omer);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    [MenuItem("TLTools/GS Config Ali", false, 42)]
    public static void SetGamesparksEnvAli()
    {
        LogUtil.Log("Switch to GS Config Ali", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.Ali);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    [MenuItem("TLTools/GS Config Maryam", false, 43)]
    public static void SetGamesparksEnvMaryam()
    {
        LogUtil.Log("Switch to GS Config Maryam", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.Maryam);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    public static void SetGamesparksEnvURLBased()
    {
        LogUtil.Log("Switch to GS Config URLBased", "yellow");
        bool status = SetGameSparksEnvironment(GameSparksConfig.Environment.URLBased);

        if (status == false)
        {
            LogUtil.Log("Error: Failed to switch gamesparks environment!", "red");
        }
    }

    private static GameObject FindSceneRootGameObject(Scene scene, string name)
    {
        GameObject[] rootObjs = scene.GetRootGameObjects();

        bool found = false;
        int i = 0;
        while (!found && i < rootObjs.Length)
        {
            found = rootObjs[i].name == name;
            if (!found) i++;
        }

        return found ? rootObjs[i] : null;
    }

    private static bool SetGameSparksEnvironment(GameSparksConfig.Environment env)
    {
        Validate();
        int currSceneIndexCopy = currSceneIndex;
        Scene scene = SwitchScene(1);
        GameObject gameSparksConfig = FindSceneRootGameObject(scene, "GameSparksConfig");

        bool isSet = false;
        if (gameSparksConfig != null)
        {
            GameSparksConfig configScript = gameSparksConfig.GetComponent<GameSparksConfig>();
            configScript.environment = env;
            EditorSceneManager.SaveScene(scene);
            isSet = true;
        }

        SwitchScene(currSceneIndexCopy);
        return isSet;
    }

    private static GameSparksConfig.Environment GetGameSparksEnvironment()
    {
        Validate();
        int currSceneIndexCopy = currSceneIndex;
        Scene scene = SwitchScene(1);
        GameObject gameSparksConfig = FindSceneRootGameObject(scene, "GameSparksConfig");
        GameSparksConfig.Environment env = GameSparksConfig.Environment.Unassigned;

        if (gameSparksConfig != null)
        {
            GameSparksConfig configScript = gameSparksConfig.GetComponent<GameSparksConfig>();
            env = configScript.environment;
        }

        SwitchScene(currSceneIndexCopy);
        return env;
    }

    private static Scene SwitchScene(int index)
    {
        Validate();
        Scene scene = EditorSceneManager.GetActiveScene();
        LogUtil.Log("SwitchScene gameSceneFiles index" + index);

        if (index >= 0 && scene.path != gameSceneFiles[index])
        {
            scene = EditorSceneManager.OpenScene(gameSceneFiles[index]);
        }

        currSceneIndex = index;
        return scene;
    }
}
