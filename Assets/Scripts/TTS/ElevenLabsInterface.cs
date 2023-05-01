using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Newtonsoft.Json;

public class ElevenLabsInterface : TTSInterface
{
    //Unity Stuff
    public SoundCharacterAnimator soundCharacterAnimator;

    public float stability = 0.1f;
    public float similarity = 0.8f;

    private string apiKey = "";
    private string voiceID = "";
    private string speechSynthesisEndpoint = "https://api.elevenlabs.io/v1/text-to-speech/";


    [System.Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public string model_id;
        public VoiceSettings voice_settings;
    }

    [System.Serializable]
    public class VoiceSettings
    {
        public float stability;
        public float similarity_boost;
    }


    private void Awake()
    {
        onTTSResponse = new UnityEngine.Events.UnityEvent<AudioClip>();
    }

    public override void Init()
    {
        base.Init();
        this.Init("");
    }

    public void Init(string key = "", string voiceID = "")
    {
        if (key == "")
        {
            Debug.LogError("API Key is required to use ElevenLabsInterface");
            return;
        }

        if(voiceID == "")
        {
            Debug.LogError("Voice ID is required to use ElevenLabsInterface");
            return;
        }

        apiKey = key;
        this.voiceID = voiceID;
    }

    public override async Task SendRequest(string request)
    {
        var ttsObservable = ConvertTextToSpeech(request);
        var tcs = new TaskCompletionSource<AudioClip>();

        ttsObservable.Subscribe(
            audioClip =>
            {
                tcs.SetResult(audioClip);
            },
            ex =>
            {
                tcs.SetException(ex);
            },
            () =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    tcs.SetCanceled();
                }
            });

        try
        {
            AudioClip audioClip = await tcs.Task;
            await ResponseHandle(audioClip);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }

    public override Task ResponseHandle(AudioClip response)
    {
        soundCharacterAnimator.PlayAudioClip(response);
        return Task.CompletedTask;
    }


    public IObservable<AudioClip> ConvertTextToSpeech(string text)
    {
        return Observable.FromCoroutine<AudioClip>(observer => DownloadAudioClip(observer, text));
    }

    private IEnumerator DownloadAudioClip(IObserver<AudioClip> observer, string text)
    {
        string url = speechSynthesisEndpoint + voiceID;
        Debug.Log("Eleven Labs TTS Request: " + url);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("accept", "audio/mpeg");
        request.SetRequestHeader("xi-api-key", apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        string requestBody = JsonConvert.SerializeObject(new TextToSpeechRequest
        {
            text = text,
            model_id = "eleven_multilingual_v1",
            voice_settings = new VoiceSettings
            {
                stability = this.stability,
                similarity_boost = this.similarity
            }
        });

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);

        Debug.Log(requestBody);

        request.SendWebRequest();

        Debug.Log("Eleven Labs TTS Request Send!");

        while (!request.isDone)
        {
            yield return null;
        }



        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.uploadHandler != null) request.uploadHandler.Dispose();
            if (request.downloadHandler != null) request.downloadHandler.Dispose();

            observer.OnError(new Exception(request.error));
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);

            if (request.uploadHandler != null) request.uploadHandler.Dispose();
            if (request.downloadHandler != null) request.downloadHandler.Dispose();

            observer.OnNext(audioClip);
            observer.OnCompleted();
        }


    }

}
