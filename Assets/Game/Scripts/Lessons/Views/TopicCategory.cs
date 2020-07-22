using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class TopicCategory : MonoBehaviour
{
    public Text title;
    public Transform topicTileContainer;
    public SkinLink skinLink;

    public void Init(string title, List<TopicVO> topics, GameObjectsPool topicTilePool)
    {
        this.title.text = title;
        skinLink.InitPrefabSkin();

        foreach (var topic in topics)
        {
            var tile = topicTilePool.GetObject();
            tile.transform.SetParent(topicTileContainer);
            tile.GetComponent<TopicTile>().Init(topic);
            tile.SetActive(true);
        }
    }
}
