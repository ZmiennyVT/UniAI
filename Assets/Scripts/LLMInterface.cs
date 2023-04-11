using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChatGPTWrapper;
using UniRx;
public class LLMInterface : MonoBehaviour
{
    public TTSInterface ttsInterface;
    public GameObject waifuMessagePrefab;
    public GameObject waifuMessageContainer;

    public ChatGPTConversation chatGPTConversation;
    public TMP_InputField chatTextInput;
    public void ResponseHandle(string response)
    {
        WaifuChatInterfaceManager.instance.AddMessage(response, false);
        Debug.Log("Response: " + response);

        ttsInterface.ConvertTextToSpeech(response)
            .Subscribe(
                audioClip => ttsInterface.PlayAudioClip(audioClip),
                error => Debug.LogError("B³¹d: " + error.Message))
            .AddTo(this);

        ttsInterface.Progress
            .Subscribe(progress => Debug.Log("Progress: " + progress))
            .AddTo(this);
    }

    public void SendToLLM(string message)
    {
        WaifuChatInterfaceManager.instance.AddMessage(message, true);
        chatGPTConversation.SendToChatGPT(message);
    }
}
