/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-13 17:28:08 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;

using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.InstantFramework;

namespace TurboLabz.Chess
{
    public class ObjectPool
    {
        Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
        Dictionary<string, List<GameObject>> used = new Dictionary<string, List<GameObject>>();

        public void AddObject(GameObject obj)
        {
            string name = obj.name;
            obj.SetActive(false);
            List<GameObject> objList = null;

            if (pool.ContainsKey(name))
            {
                objList = pool[name];
                objList.Add(obj);
            }
            else
            {
                objList = new List<GameObject>();
                objList.Add(obj);
                pool.Add(name, objList);

                List<GameObject> usedList = new List<GameObject>();
                used.Add(name, usedList);
            }
        }

        public GameObject GetObject(string name)
        {
            if (pool.ContainsKey(name))
            {
                List<GameObject> objList = pool[name];

                if (objList.Count == 0)
                {
                    List<GameObject> usedObjList = used[name];
                    GameObject cloneObj = GameObject.Instantiate(usedObjList[0]);

                    //fix for promo pieces not scaleing properly
                    //old code: cloneObj.transform.parent = usedObjList[0].transform.parent;
                    //new code, instead setting its parent directly, used SetParent method and
                    //set its worldPositionStays parameter to false so its scale will not be adjusted by the parent
                    cloneObj.transform.SetParent(usedObjList[0].transform.parent, false);

                    cloneObj.name = name;
                    usedObjList.Add(cloneObj);

                    // TODO: this is a skin link hack and this dependency does not
                    // belong here. we need to figure out how to register cloned
                    // object mediators for their skinlinks to receive update signals.
                    // for now its a strange ioc issue.
                    SkinLink link = usedObjList[0].GetComponent<SkinLink>();
                    if (link != null) link.AddClone(cloneObj.GetComponent<SkinLink>());

                    return cloneObj;
                }
                else
                {
                    GameObject obj = objList[0];
                    List<GameObject> usedObjList = used[name];
                    usedObjList.Add(obj);
                    objList.RemoveAt(0);
                    obj.SetActive(true);
                    return obj;
                }
            }
            else
            {
                Assertions.Assert(false, "Object was never added to the pool.");
                return null;
            }
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);

            if (used.ContainsKey(obj.name))
            {
                List<GameObject> usedObjList = used[obj.name];
                if (usedObjList.Contains(obj))
                {
                    usedObjList.Remove(obj);
                    pool[obj.name].Add(obj);
                }
                else
                {
                    Assertions.Assert(false, "This object was never part of the pool.");
                }
            }
            else
            {
                Assertions.Assert(false, "Object pool does not exist for this object.");
            }
        }

        public int GetUsedPoolSize(string name)
        {
            return used[name].Count;
        }

        public int GetPoolSize(string name)
        {
            return pool[name].Count;
        }

        public static void Test()
        {
            ObjectPool pool = new ObjectPool();

            // Create 3 fruits
            GameObject banana = new GameObject("banana");
            GameObject apple = new GameObject("apple");
            GameObject orange = new GameObject("orange");

            // Add 10 of each to the pool
            for (int i = 0; i < 10; ++i)
            {
                GameObject obj = GameObject.Instantiate(banana);
                obj.name = banana.name;
                pool.AddObject(obj);

                obj = GameObject.Instantiate(apple);
                obj.name = apple.name;
                pool.AddObject(obj);

                obj = GameObject.Instantiate(orange);
                obj.name = orange.name;
                pool.AddObject(obj);
            }

            // Verify pool sizes
            Assertions.Assert(pool.GetPoolSize("banana") == 10, "Banana pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("apple") == 10, "Apple pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("orange") == 10, "Orange pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("banana") == 0, "Banana used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("apple") == 0, "Apple used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("orange") == 0, "Orange used pool size invalid.");

            // Take 5 apples from pool
            for (int i = 0; i < 5; ++i)
            {
                pool.GetObject("apple");
            }

            // Verify pool sizes
            Assertions.Assert(pool.GetPoolSize("banana") == 10, "Banana pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("apple") == 5, "Apple pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("orange") == 10, "Orange pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("banana") == 0, "Banana used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("apple") == 5, "Apple used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("orange") == 0, "Orange used pool size invalid.");

            // Take 4 oranges from pool 
            List<GameObject> usedOranges = new List<GameObject>();
            for (int i = 0; i < 4; ++i)
            {
                usedOranges.Add(pool.GetObject("orange"));
            }

            // Verify pool sizes
            Assertions.Assert(pool.GetPoolSize("banana") == 10, "Banana pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("apple") == 5, "Apple pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("orange") == 6, "Orange pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("banana") == 0, "Banana used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("apple") == 5, "Apple used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("orange") == 4, "Orange used pool size invalid.");

            // Return 2 oranges to pool
            for (int i = 0; i < 2; ++i)
            {
                pool.ReturnObject(usedOranges[i]);
                usedOranges.RemoveAt(0);
            }

            // Verify pool sizes
            Assertions.Assert(pool.GetPoolSize("banana") == 10, "Banana pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("apple") == 5, "Apple pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("orange") == 8, "Orange pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("banana") == 0, "Banana used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("apple") == 5, "Apple used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("orange") == 2, "Orange used pool size invalid.");

            // Take 12 bananas from pool
            List<GameObject> usedBananas = new List<GameObject>();
            for (int i = 0; i < 12; ++i)
            {
                usedBananas.Add(pool.GetObject("banana"));
            }

            // Verify pool sizes
            Assertions.Assert(pool.GetPoolSize("banana") == 0, "Banana pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("apple") == 5, "Apple pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("orange") == 8, "Orange pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("banana") == 12, "Banana used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("apple") == 5, "Apple used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("orange") == 2, "Orange used pool size invalid.");

            // Return 12 bananas to pool
            for (int i = 0; i < 12; ++i)
            {
                pool.ReturnObject(usedBananas[i]);
            }

            usedBananas.Clear();

            // Verify pool sizes
            Assertions.Assert(pool.GetPoolSize("banana") == 12, "Banana pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("apple") == 5, "Apple pool size invalid.");
            Assertions.Assert(pool.GetPoolSize("orange") == 8, "Orange pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("banana") == 0, "Banana used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("apple") == 5, "Apple used pool size invalid.");
            Assertions.Assert(pool.GetUsedPoolSize("orange") == 2, "Orange used pool size invalid.");
        }
    }
}
