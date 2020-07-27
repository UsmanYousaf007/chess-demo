using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class LessonTile : MonoBehaviour
{
    public Text lessonName;
    public GameObject locked;
    public Button button;
    public SkinLink skinLink;
    public GameObject completed;
    public Image progress;

    [HideInInspector] public VideoLessonVO vo;

    public void Init(VideoLessonVO vo)
    {
        this.vo = vo;
        lessonName.text = $"{vo.indexInTopic}. {vo.name}";
        locked.SetActive(vo.isLocked);
        skinLink.InitPrefabSkin();
        progress.fillAmount = vo.progress;
        completed.SetActive(vo.progress >= 1);
    }

    public void Unlock()
    {
        vo.isLocked = false;
        locked.SetActive(false);
    }
}
