﻿using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class GameObjectsPool
    {
        private GameObject _poolParent;
        private GameObject _prefab;
        private List<GameObject> _pool;

        public GameObjectsPool(GameObject friendBarPrefab, int initialPoolCount = 0)
        {
            _prefab = friendBarPrefab;

            _poolParent = new GameObject(friendBarPrefab.name + "_pool");

            if (initialPoolCount > 0)
            {
                _pool = new List<GameObject>(initialPoolCount + 1);

                for (int i = 0; i < initialPoolCount; i++)
                {
                    GameObject obj = GameObject.Instantiate(_prefab);
                    ReturnObject(obj);
                }
            }
            else
            {
                _pool = new List<GameObject>();
            }
        }

        public GameObject GetObject()
        {
            GameObject retObj = null;

            // If we have an object in pool then we use that, else we instantiate a new bar
            if (_pool.Count > 0)
            {
                // Removing from end of list because it's fast
                int listEndIndex = _pool.Count - 1;
                retObj = _pool[listEndIndex];
                _pool.RemoveAt(listEndIndex);

                // Removing as child of _poolParent
                retObj.transform.SetParent(null, false);
            }
            else
            {
                retObj = GameObject.Instantiate(_prefab);
            }

            return retObj;
        }

        public void ReturnObject(GameObject obj)
        {
            if (_pool == null)
                _pool = new List<GameObject>();

            _pool.Add(obj);
            obj.transform.SetParent(_poolParent.transform, false);
            obj.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            _pool = null;
            GameObject.Destroy(_poolParent);
        }

        ~GameObjectsPool()
        {
            Destroy();
        }
    }
}