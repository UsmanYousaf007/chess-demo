using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class FriendBarsPool
    {
        private GameObject _poolParent;
        private GameObject _friendBarPrefab;
        private List<FriendBar> _friendBarsPool = new List<FriendBar>();

        public FriendBarsPool(GameObject friendBarPrefab, int initialPoolCount = 0)
        {
            _friendBarPrefab = friendBarPrefab;

            _poolParent = new GameObject("FriendBarsPool");

            if (initialPoolCount > 0)
            {
                _friendBarsPool = new List<FriendBar>(initialPoolCount + 1);

                for (int i = 0; i < initialPoolCount; i++)
                {
                    FriendBar friendBar = GameObject.Instantiate(_friendBarPrefab).GetComponent<FriendBar>();
                    ReturnBar(friendBar);
                }
            }
            else
            {
                _friendBarsPool = new List<FriendBar>();
            }
        }

        public FriendBar GetBar()
        {
            FriendBar friendBar = null;

            if (_friendBarsPool.Count > 0)
            {
                // Removing from end of list because it's fast
                int listEndIndex = _friendBarsPool.Count - 1;
                friendBar = _friendBarsPool[listEndIndex];
                _friendBarsPool.RemoveAt(listEndIndex);

                // Removing as child of _poolParent
                friendBar.transform.SetParent(null, false);
            }
            else
            {
                friendBar = GameObject.Instantiate(_friendBarPrefab).GetComponent<FriendBar>();
            }

            return friendBar;
        }

        public void ReturnBar(FriendBar friendBar)
        {
            if (_friendBarsPool == null)
                _friendBarsPool = new List<FriendBar>();

            _friendBarsPool.Add(friendBar);
            friendBar.transform.SetParent(_poolParent.transform, false);
            friendBar.gameObject.SetActive(false);
        }

        ~FriendBarsPool()
        {
            _friendBarsPool = null;
            GameObject.Destroy(_poolParent);
        }
    }
}
