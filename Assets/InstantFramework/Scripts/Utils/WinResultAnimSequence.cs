using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinResultAnimSequence : MonoBehaviour
{
    [SerializeField] private GameObject _iconSparklesFXObj;
    [SerializeField] private GameObject _iconRaysFXObj;
    [SerializeField] private RewardParticleEmitter _coinsParticleEmitter;
    [SerializeField] private RewardParticleEmitter _starsParticleEmitter;
    [SerializeField] private RewardParticleEmitter _powerPlayParticleEmitter;
    [SerializeField] private ParticleSystem _powerPlayFX;
    [SerializeField] private Text _coinsText;
    [SerializeField] private Text _starsText;
    [SerializeField] private GameObject _viewBoardBtnObj;
    [SerializeField] private Button _ratingBoosterBtn;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private GameObject _crossPromoBtnObj;
    [SerializeField] private GameObject [] _coinPanelChildObjects;
    [SerializeField] private GameObject [] _starsPanelChildObjects;
    [SerializeField] private Image powerPlayImage;

    private Animator _animator;

    private long _coinsRewarded = 0;
    private int _starsRewarded = 0;
    private int _powerPlayBonus = 0;

    public void Reset(long coinsRewarded, int starsRewarded, int powerPlayBonus = 0, bool playerWon = false)
    {
        _iconSparklesFXObj.SetActive(false);
        _iconRaysFXObj.SetActive(false);

        _coinsParticleEmitter.gameObject.SetActive(false);
        _coinsRewarded = coinsRewarded;

        _starsParticleEmitter.gameObject.SetActive(false);
        _starsRewarded = starsRewarded;

        _powerPlayParticleEmitter.gameObject.SetActive(false);
        _powerPlayFX.gameObject.SetActive(false);

        _powerPlayBonus = powerPlayBonus;

        _coinsText.text = "0";
        _starsText.text = "0";

        _viewBoardBtnObj.SetActive(!playerWon);
        _crossPromoBtnObj.SetActive(!playerWon);
        _ratingBoosterBtn.interactable = !playerWon;
        _continueBtn.interactable = !playerWon;
        _continueBtn.gameObject.SetActive(!playerWon);
        powerPlayImage.enabled = playerWon;

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.enabled = false;

        for (int i = 0; i < _coinPanelChildObjects.Length; i++)
        {
            _coinPanelChildObjects[i].SetActive(false);
        }

        for (int i = 0; i < _starsPanelChildObjects.Length; i++)
        {
            _starsPanelChildObjects[i].SetActive(false);
        }

        _crossPromoBtnObj.SetActive(!playerWon);
    }

    public void PlayAnimation()
    {
        _animator.enabled = true;

        for (int i = 0; i < _coinPanelChildObjects.Length; i++)
        {
            _coinPanelChildObjects[i].SetActive(true);
        }

        for (int i = 0; i < _starsPanelChildObjects.Length; i++)
        {
            _starsPanelChildObjects[i].SetActive(true);
        }
    }

    private void PlayCoinEffect()
    {
        _coinsParticleEmitter.gameObject.SetActive(true);
        _coinsParticleEmitter.PlayFx();

        //_coinsText.text = "x0";
        _coinsText.gameObject.SetActive(true);
        iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", 0,
                    "to", (int)_coinsRewarded,
                    "time", 0.5f,
                    "delay", 1.15f,
                    "onupdate", "OnCointCountUpdate",
                    "onupdatetarget", this.gameObject
                //"oncomplete", "AnimationComplete"
                ));
    }

    private void OnCointCountUpdate(int val)
    {
        _coinsText.text = val.ToString("N0");
    }

    private void PlayStarsEffect()
    {
        _starsParticleEmitter.gameObject.SetActive(true);
        _starsParticleEmitter.PlayFx();

        //_starsText.text = "x0";
        _starsText.gameObject.SetActive(true);
        iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", 0,
                    "to", _starsRewarded,
                    "time", 0.5f,
                    "delay", 1.15f,
                    "onupdate", "OnStarsCountUpdate",
                    "onupdatetarget", this.gameObject
                //"oncomplete", "AnimationComplete"
                ));
    }

    private void OnStarsCountUpdate(int val)
    {
        _starsText.text = val.ToString("N0");
    }

    private void PlayPowerPlayEffect()
    {
        if (_powerPlayBonus > 0)
        {
            _powerPlayParticleEmitter.gameObject.SetActive(true);
            _powerPlayParticleEmitter.PlayFx();

            _powerPlayFX.gameObject.SetActive(true);
            _powerPlayFX.Play();

            //_starsText.text = "x0";
            _starsText.gameObject.SetActive(true);
            iTween.ValueTo(this.gameObject,
                    iTween.Hash(
                        "from", _starsRewarded,
                        "to", _starsRewarded + _powerPlayBonus,
                        "time", 0.5f,
                        "delay", 1.15f,
                        "onupdate", "OnStarsCountUpdate",
                        "onupdatetarget", this.gameObject,
                        "oncomplete", "OnStarsCountAnimationComplete"
                    ));
        }
        else
        {
            _viewBoardBtnObj.SetActive(true);
            _crossPromoBtnObj.SetActive(true);
            _ratingBoosterBtn.interactable = true;
            _continueBtn.interactable = true;
            _continueBtn.gameObject.SetActive(true);
            powerPlayImage.enabled = false;

            _animator.enabled = false;

            TweenInCrossPromo();
        }
    }

    private void OnStarsCountAnimationComplete()
    {
        _viewBoardBtnObj.SetActive(true);
        _crossPromoBtnObj.SetActive(true);
        _ratingBoosterBtn.interactable = true;
        _continueBtn.interactable = true;
        _continueBtn.gameObject.SetActive(true);
        powerPlayImage.enabled = false;

        _animator.enabled = false;

        TweenInCrossPromo();
    }

    private void TweenInCrossPromo()
    {
        Vector3 crossPromoTransformPosition = _crossPromoBtnObj.transform.position;
        float crossPromoY = crossPromoTransformPosition.y;
        _crossPromoBtnObj.transform.position = new Vector3(crossPromoTransformPosition.x, -349f, crossPromoTransformPosition.z);
        _crossPromoBtnObj.SetActive(true);
        _crossPromoBtnObj.transform.DOMoveY(crossPromoY, 0.2f);
    }
}
