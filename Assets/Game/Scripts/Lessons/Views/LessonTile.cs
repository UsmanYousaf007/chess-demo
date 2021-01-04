using System.Collections;
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
    public GameObject play;
    public Button unlockBtn;
    public Text price;
    public ParticleSystem unlockedAnimation;

    [HideInInspector] public VideoLessonVO vo;
    [HideInInspector] public bool haveEnoughItemsToUnlock;
    [HideInInspector] public bool haveEnoughGemsToUnlock;

    private void OnEnable()
    {
        unlockedAnimation.gameObject.SetActive(false);
    }

    public void Init(VideoLessonVO vo)
    {
        this.vo = vo;
        lessonName.text = $"{vo.indexInTopic}. {vo.name}";
        locked.SetActive(vo.isLocked);
        play.SetActive(!vo.isLocked);
        unlockBtn.gameObject.SetActive(vo.isLocked);
        skinLink.InitPrefabSkin();
        progress.fillAmount = vo.progress;
        completed.SetActive(vo.progress >= 1);
        SetupUnlockButton();
    }

    public void Unlock()
    {
        if (vo.isLocked)
        {
            PlayAnimation();
        }

        vo.isLocked = false;
        locked.SetActive(false);
        play.SetActive(true);
        unlockBtn.gameObject.SetActive(false);
    }

    public void SetupUnlockButton()
    {
        if (vo.storeItem == null)
        {
            return;
        }

        haveEnoughGemsToUnlock = vo.playerModel.gems >= vo.storeItem.currency3Cost;
        price.text = vo.storeItem.currency3Cost.ToString();
    }

    private void PlayAnimation()
    {
        unlockedAnimation.gameObject.SetActive(true);
        StartCoroutine(StopAnimation());
    }

    IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(2.0f);
        unlockedAnimation.gameObject.SetActive(false);
    }
}
