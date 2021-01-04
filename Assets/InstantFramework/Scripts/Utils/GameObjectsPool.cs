using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class GameObjectsPool
    {
        private static Dictionary<string, List<GameObject>> POOLS_LIST = new Dictionary<string, List<GameObject>>();
        private static Dictionary<string, int> POOL_COUNT = new Dictionary<string, int>();

        private GameObject _poolParent;
        private GameObject _prefab;
        private List<GameObject> _pool;

        public GameObjectsPool(GameObject prefab, int initialPoolCount = 0)
        {
            _prefab = prefab;

            string poolName = prefab.name + "_pool";
            if (POOLS_LIST.ContainsKey(poolName))
            {
                _poolParent = GameObject.Find(poolName);
                _pool = POOLS_LIST[poolName];
                POOL_COUNT[poolName]++;
            }
            else
            {
                _poolParent = new GameObject(poolName);

                if (initialPoolCount > 0)
                {
                    _pool = new List<GameObject>(initialPoolCount);

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

                POOLS_LIST.Add(poolName, _pool);
                POOL_COUNT.Add(poolName, 1);
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
            string poolName = _prefab.name + "_pool";
            POOLS_LIST.Remove(poolName);
            POOL_COUNT[poolName]--;
            if (POOL_COUNT[poolName] == 0)
            {
                _pool = null;
                GameObject.Destroy(_poolParent);
            }
        }

        ~GameObjectsPool()
        {
            Destroy();
        }
    }
}
