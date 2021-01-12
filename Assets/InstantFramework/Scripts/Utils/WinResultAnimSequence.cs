using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinResultAnimSequence : MonoBehaviour
{
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

    private Animator _animator;

    private long _coinsRewarded = 0;
    private int _starsRewarded = 0;
    private int _powerPlayBonus = 0;

    public void Init(long coinsRewarded, int starsRewarded, int powerPlayBonus = 0)
    {
        _coinsParticleEmitter.gameObject.SetActive(false);
        _coinsRewarded = coinsRewarded;

        _starsParticleEmitter.gameObject.SetActive(false);
        _starsRewarded = starsRewarded;

        _powerPlayBonus = powerPlayBonus;

        _viewBoardBtnObj.SetActive(false);
        _crossPromoBtnObj.SetActive(false);
        _ratingBoosterBtn.interactable = false;
        _continueBtn.interactable = false;

        _crossPromoBtnObj.transform.position = new Vector3(0f, -349f, 1f);

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.enabled = false;
    }

    public void PlayAnimation()
    {
        _animator.enabled = true;
        //_animator.pla
    }

    private void PlayCoinEffect()
    {
        _coinsParticleEmitter.gameObject.SetActive(true);
        _coinsParticleEmitter.PlayFx();

        _coinsText.text = "x0";
        _coinsText.gameObject.SetActive(true);
        iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", 0,
                    "to", _coinsRewarded,
                    "time", 0.75f,
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

        _starsText.text = "x0";
        _starsText.gameObject.SetActive(true);
        iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", 0,
                    "to", _starsRewarded,
                    "time", 0.75f,
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

            _starsText.text = "x0";
            _starsText.gameObject.SetActive(true);
            iTween.ValueTo(this.gameObject,
                    iTween.Hash(
                        "from", _starsRewarded,
                        "to", _starsRewarded + _powerPlayBonus,
                        "time", 0.75f,
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

            TweenInCrossPromo();
        }
    }

    private void OnStarsCountAnimationComplete()
    {
        _viewBoardBtnObj.SetActive(true);
        _crossPromoBtnObj.SetActive(true);
        _ratingBoosterBtn.interactable = true;
        _continueBtn.interactable = true;

        TweenInCrossPromo();
    }

    private void TweenInCrossPromo()
    {
        _crossPromoBtnObj.transform.DOMoveY(349f, 0.2f);
    }
}
