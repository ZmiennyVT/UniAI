using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

//TODO: This is a temporary solution
public class SoundCharacterAnimator : MonoBehaviour
{
    public AudioSource audioSource;
    public Animator animator;

    private void Start()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => {
                animator.SetBool("talking", audioSource.isPlaying);
                animator.SetFloat("idle_blend", 1f * Mathf.PerlinNoise(Time.time * 0.2f, 0.0f));
            })
            .AddTo(this);
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        animator.SetFloat("talking_blend", UnityEngine.Random.RandomRange(0f, 1f));
        audioSource.Play();
    }
}
