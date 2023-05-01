using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public LLMInterface llmInterface;
    public TTSInterface ttsInterface;


    public static AppManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.LogError("Multiple AppManagers in scene!");
            Destroy(this);
        }
    }

    private void Start()
    {
        //TODO: Refactor this spaghetii
        llmInterface.onLLMResponse.AddListener(async (llmResponse) =>
        {
            SendRequestToTTSInteface(llmResponse);
            WaifuChatInterfaceManager.instance.AddMessage(llmResponse, false);
        });

        if (APIManager.instance.LoadAPIData())
        {
            UpdateAPISettings();
        }
    }

    public void SendRequestToTTSInteface(string request)
    {
        StartCoroutine(HandleTTSRequest(request));
    }

    private IEnumerator HandleTTSRequest(string request)
    {
        Task task = ttsInterface.SendRequest(request);
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("Task TTS Completed");
    }

    public void SendLLMRequest(string request)
    {
        Debug.Log("LLM Request: " + request);
        StartCoroutine(HandleLLMRequest(request));
    }

    public void UpdateAPISettings()
    {
        WaifuChatInterfaceManager.instance.UpdateSettingsView(APIManager.instance.apiData);
        //Temp solution
        ((OpenAIInterface)llmInterface).Init(APIManager.instance.apiData.openAIAPIKey, APIManager.instance.apiData.openAIInitialPrompt);
        ((ElevenLabsInterface)ttsInterface).Init(APIManager.instance.apiData.elevenLabsAPIKey, APIManager.instance.apiData.elevenLabsVoiceID);
        ((ElevenLabsInterface)ttsInterface).similarity = APIManager.instance.apiData.elevenLabsSimilarity;
        ((ElevenLabsInterface)ttsInterface).stability = APIManager.instance.apiData.elevenLabsStability;
    }

    private IEnumerator HandleLLMRequest(string request)
    {
        Task task = llmInterface.SendRequest(request);
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("Task LLM Completed");
    }

}
