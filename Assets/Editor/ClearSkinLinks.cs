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

public class ClearSkinLinks : EditorWindow 
{
    [MenuItem ("Tools/TurboLabz/Clear SkinLinks")]
    public static void Init()
    {  
        ClearSkinLinks c = ScriptableObject.CreateInstance<ClearSkinLinks>();
        c.DoClearSkinLinks();
    }

    void DoClearSkinLinks()
    {
        object[] objects = FindObjects<SkinLink>();
        bool markDirty = false;
        foreach (object obj in objects)
        {
            GameObject gameObject = (GameObject)obj;
            Image imageObject = gameObject.GetComponent<Image>();
            //if (imageObject == null)
            //{
            //    Debug.Log(gameObject.transform.parent.name + "=>" + gameObject.name + " --- Object with skinlink but no image component!");
            //}
            //else 
            if (imageObject && imageObject.sprite != null)
            {
                Debug.Log(gameObject.transform.parent.name + "=>" + gameObject.name + " Image:" + imageObject.sprite.name);

                imageObject.sprite = null;
                markDirty = true;
            }
        }

        if (markDirty)
        {
            var scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }

        Debug.Log("Process complete");
    }

    private GameObject[] FindObjects<T>() where T : Object
    {
        List<GameObject> gameObjects = new List<GameObject>();
        GameObject[] rootObjects = GetRootGameObjects();

        foreach (GameObject gameObject in rootObjects) 
        {
            Transform[] transformList = gameObject.GetComponentsInChildren<Transform>(true).ToArray();
            foreach (Transform tr in transformList) 
            {
                if (tr.GetComponent<T>())
                {
                    gameObjects.Add(tr.gameObject);
                }
            }
        }
        return (GameObject[])gameObjects.ToArray();
    }

    private static GameObject[] GetRootGameObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int sceneIdx = 0; sceneIdx < UnityEngine.SceneManagement.SceneManager.sceneCount; ++sceneIdx)
        {
            gameObjects.AddRange( UnityEngine.SceneManagement.SceneManager.GetSceneAt(sceneIdx).GetRootGameObjects().ToArray() );
        }
        return gameObjects.ToArray();
    }
}