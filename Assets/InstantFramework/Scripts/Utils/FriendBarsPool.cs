using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class FriendBarsPool
    {
        private GameObject _friendBarPrefab;
        private List<FriendBar> friendBarsPool = new List<FriendBar>();

        public FriendBarsPool(GameObject friendBarPrefab, int initialPoolCount = 0)
        {
            _friendBarPrefab = friendBarPrefab;

            if (initialPoolCount > 0)
            {
                friendBarsPool = new List<FriendBar>(initialPoolCount + 1);

                for (int i = 0; i < initialPoolCount; i++)
                {
                    FriendBar friendBar = GameObject.Instantiate(_friendBarPrefab).GetComponent<FriendBar>();
                    friendBar.gameObject.SetActive(false);
                    ReturnBar(friendBar);
                }
            }
            else
            {
                friendBarsPool = new List<FriendBar>();
            }
        }

        public FriendBar GetBar()
        {
            FriendBar friendBar = null;

            if (friendBarsPool.Count > 0)
            {
                int listEndIndex = friendBarsPool.Count - 1;
                friendBar = friendBarsPool[listEndIndex];
                friendBarsPool.RemoveAt(listEndIndex);
            }
            else
            {
                friendBar = GameObject.Instantiate(_friendBarPrefab).GetComponent<FriendBar>();
            }

            return friendBar;
        }

        public void ReturnBar(FriendBar friendBar)
        {
            if (friendBarsPool == null)
                friendBarsPool = new List<FriendBar>();

            friendBarsPool.Add(friendBar);
        }
    }
}
