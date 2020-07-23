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
    public SkinLink skinLink;

    public void Init(string title, List<TopicVO> topics, GameObjectsPool topicTilePool, Signal<TopicVO> onClickSignal)
    {
        this.title.text = title;
        skinLink.InitPrefabSkin();

        foreach (var topic in topics)
        {
            var tile = topicTilePool.GetObject();
            var topicTile = tile.GetComponent<TopicTile>();
            topicTile.Init(topic);
            topicTile.button.onClick.RemoveAllListeners();
            topicTile.button.onClick.AddListener(() => onClickSignal.Dispatch(topic));
            tile.transform.SetParent(topicTileContainer, false);
            tile.SetActive(true);
        }
    }
}
