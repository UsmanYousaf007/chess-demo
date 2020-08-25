using System.Collections.Generic;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class TopicCategory : MonoBehaviour
{
    public Text title;
    public Transform topicTileContainer;
    public SkinLink[] skinLinks;
    public Transform emptyTile;

    private GameObjectsPool tilePool;
    private GameObjectsPool rowPool;

    public void Init(string title, List<TopicVO> topics, GameObjectsPool gridRowPool, GameObjectsPool topicTilePool, Signal<TopicVO> onClickSignal)
    {
        this.title.text = title;

        foreach (var skinLink in skinLinks)
        {
            skinLink.InitPrefabSkin();
        }

        int i = 0;
        GameObject topicContainer = null;

        foreach (var topic in topics)
        {
            if (i % 2 == 0)
            {
                topicContainer = gridRowPool.GetObject();
                topicContainer.SetActive(true);
                topicContainer.transform.SetParent(topicTileContainer, false);
            }

            var tile = topicTilePool.GetObject();
            var topicTile = tile.GetComponent<TopicTile>();
            topicTile.Init(topic);
            topicTile.button.onClick.RemoveAllListeners();
            topicTile.button.onClick.AddListener(() => onClickSignal.Dispatch(topic));
            tile.transform.SetParent(topicContainer.transform, false);
            tile.SetActive(true);
            tilePool = topicTilePool;
            rowPool = gridRowPool;
            i++;

            if (i == topics.Count)
            {
                emptyTile.gameObject.SetActive(i % 2 == 1);
                emptyTile.SetParent(topicContainer.transform, false);
            }
        }
    }

    public void Reset()
    {
        foreach (var tile in topicTileContainer.GetComponentsInChildren<TopicTile>())
        {
            tilePool.ReturnObject(tile.gameObject);
        }

        foreach (var row in topicTileContainer.GetComponentsInChildren<HorizontalLayoutGroup>())
        {
            rowPool.ReturnObject(row.gameObject);
        }
    }
}
