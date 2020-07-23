using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class TopicTile : MonoBehaviour
{
    public Image icon;
    public Image progressBar;
    public Text progressLabel;
    public Text nameLabel;
    public Text totalLessonsLabel;
    public GameObject completedIcon;
    public Button button;
    public SkinLink skinLink;

    public void Init(TopicVO vo)
    {
        skinLink.InitPrefabSkin();
        icon.sprite = vo.icon;
        icon.SetNativeSize();
        nameLabel.text = vo.name;
        totalLessonsLabel.text = $"{vo.total} Lessons";

        var completedPercentage = (float)vo.completed / vo.total;
        var isCompleted = completedPercentage == 1;
        var fillAmount = .09f + (vo.completed * ((.91f - .09f) / vo.total));
        completedIcon.SetActive(isCompleted);
        progressLabel.gameObject.SetActive(!isCompleted);
        progressBar.fillAmount = fillAmount;
        progressLabel.text = $"{(int)(completedPercentage * 100)}%";
        progressBar.color = isCompleted ? Colors.GLASS_GREEN : Colors.YELLOW;
    }
}
