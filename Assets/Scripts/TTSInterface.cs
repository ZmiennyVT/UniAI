using System;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TTSInterface : MonoBehaviour
{
    private const string ApiUrl = "https://api.elevenlabs.io/v1/text-to-speech/";
    private const string VoiceId = "YOUR_VOICE_ID";
    private const string ApiKey = "YOUR_API_KEY";
    public AudioSource audioSource;
    public Animator animator;

    private Subject<float> progressSubject = new Subject<float>();
    public IObservable<float> Progress => progressSubject;

    private void Start()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => {
                animator.SetBool("talking", audioSource.isPlaying);
                animator.SetFloat("idle_blend", 1f * Mathf.PerlinNoise(Time.time * 0.2f, 0.0f));
            })
            .AddTo(this);
    }

    public IObservable<AudioClip> ConvertTextToSpeech(string text)
    {
        return Observable.FromCoroutine<AudioClip>(observer => DownloadAudioClip(observer, text));
    }

    private IEnumerator DownloadAudioClip(IObserver<AudioClip> observer, string text)
    {
        string url = ApiUrl + VoiceId;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("xi-api-key", ApiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        string requestBody = JsonUtility.ToJson(new TextToSpeechRequest
        {
            text = text,
            voice_settings = new VoiceSettings
            {
                stability = 0.1f,
                similarity_boost = 0.8f
            }
        });

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);

        request.SendWebRequest();

        while (!request.isDone)
        {
            progressSubject.OnNext(request.downloadProgress);
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            observer.OnError(new Exception(request.error));
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            observer.OnNext(audioClip);
            observer.OnCompleted();
        }
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        animator.SetFloat("talking_blend", UnityEngine.Random.RandomRange(0f, 1f));
        audioSource.Play();
    }

    [System.Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public VoiceSettings voice_settings;
    }

    [System.Serializable]
    public class VoiceSettings
    {
        public float stability;
        public float similarity_boost;
    }
}
