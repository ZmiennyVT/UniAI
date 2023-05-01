using System;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;
using System.Threading.Tasks;

public class TTSInterface : MonoBehaviour
{
    public UnityEvent<AudioClip> onTTSResponse;

    public virtual void Init()
    {
    }

    public virtual async Task SendRequest(string request)
    {
    }
    public virtual async Task ResponseHandle(AudioClip response)
    {
    }
}
