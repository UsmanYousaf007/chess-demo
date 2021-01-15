
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using DG.Tweening;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

public class RewardParticle : View
{
    public Transform rewardObject;
    public GameObject targetObject;

    public float spreadRadius;
    Vector2 spreadPos;

    float spreadTime;
    public float minSpreadTime;
    public float maxSpreadTime;

    float spreadAirTime;
    public float minAirTime;
    public float maxAirTime;

    float flyTime;
    public float minFlyTime;
    public float maxFlyTime;

    float rotSpeed;

    public AudioClip spreadSFX;
    public AudioClip travelSFX;
    public AudioClip fillSFX;

    public Signal<AudioClip> playSFXSignal = new Signal<AudioClip>();

    void Start()
    {
        float X = Random.Range(-spreadRadius, spreadRadius);
        float Y = Random.Range(-spreadRadius, spreadRadius);
        spreadPos.Set(X+transform.position.x, Y+transform.position.y);

        spreadTime = Random.Range(minSpreadTime, maxSpreadTime);
        spreadAirTime = Random.Range(minAirTime, maxAirTime);
        flyTime = Random.Range(minFlyTime, maxFlyTime);

        rotSpeed = Random.Range(-150, 150);

        PlaySequence();
    }

    void PlaySequence()
    {
        Sequence sequence = DOTween.Sequence();
        //sequence.AppendCallback(() => playSFXSignal.Dispatch(spreadSFX));
        sequence.Append(transform.DOMove(spreadPos, spreadTime));
        sequence.AppendInterval(spreadAirTime);
        //sequence.AppendCallback(() => playSFXSignal.Dispatch(travelSFX));
        sequence.Append(transform.DOMove(targetObject.transform.position, flyTime));
        sequence.AppendCallback(EndReached);
        sequence.PlayForward();
    }

    void Update()
    {
        rewardObject.Rotate(Vector3.up * Time.deltaTime * rotSpeed);
        rewardObject.Rotate(Vector3.back * Time.deltaTime * rotSpeed);
    }

    void EndReached()
    {
        playSFXSignal.Dispatch(fillSFX);
        Destroy(gameObject);
    }
}
