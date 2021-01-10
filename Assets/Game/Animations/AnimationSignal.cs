using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationSignal : MonoBehaviour
{
    [SerializeField] private UnityEvent AnimationEventSignal;

    public void AnimationEvent()
    {
        AnimationEventSignal?.Invoke();
    }
}
