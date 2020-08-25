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
    public Text unlockText;
    public Image notEnoughUnlockItems;
    public Text requiredGems;
    public Sprite enoughGems;
    public Sprite notEnoughGems;
    public ParticleSystem unlockedAnimation;

    [HideInInspector] public VideoLessonVO vo;
    [HideInInspector] public bool haveEnoughItemsToUnlock;
    [HideInInspector] public bool haveEnoughGemsToUnlock;

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
        vo.isLocked = false;
        locked.SetActive(false);
        play.SetActive(true);
        unlockBtn.gameObject.SetActive(false);
        PlayAnimation();
    }

    public void SetupUnlockButton()
    {
        if (vo.unlockItem == null)
        {
            return;
        }

        haveEnoughItemsToUnlock = vo.playerModel.GetInventoryItemCount(vo.unlockItem.key) > 0;
        haveEnoughGemsToUnlock = vo.playerModel.gems >= vo.unlockItem.currency3Cost;
        requiredGems.text = vo.unlockItem.currency3Cost.ToString();
        notEnoughUnlockItems.gameObject.SetActive(!haveEnoughItemsToUnlock);
        notEnoughUnlockItems.sprite = haveEnoughGemsToUnlock ? enoughGems : notEnoughGems;
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
