using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaifuChatInterfaceManager : MonoBehaviour
{
    public static WaifuChatInterfaceManager instance;

    public TemplateContainer waifuMessageTemplate;
    VisualElement root;
    ScrollView waifuChatHistory;



    public struct ChatElement
    {
        public bool fromUser;
        public string message;

        public ChatElement(bool fromUser, string message)
        {
            this.fromUser = fromUser;
            this.message = message;
        }
    }

    List<ChatElement> chatHistory;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
    }

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        waifuChatHistory = root.Q<ScrollView>("ChatHistory");

        VisualElement waifuChatElement = root.Q<VisualElement>("WaifuMessage");
        VisualElement userChatElement = root.Q<VisualElement>("UserMessage");

        Button sayButton = root.Q<Button>("SayButton");
        sayButton.clicked += () => {
            string llmInpuText = root.Q<TextField>("UserInput").text;
            AppManager.instance.SendLLMRequest(llmInpuText);
            AddMessage(llmInpuText, true);
        };

        waifuChatHistory.Clear(); 


        root.Q<Button>("SettingsToggle").clicked += () => {
            SetSettingsVisibility(!root.Q<VisualElement>("SettingsPanelRoot").visible);
        }; 
        root.Q<Button>("SettingsSaveAndCloseButton").clicked += () => {
            SaveSettings();
            SetSettingsVisibility(false);
            if(!APIManager.instance.SaveAPIData()) Debug.LogError("Failed to save API data!");
            AppManager.instance.UpdateAPISettings();
        };
    }

    public void SetSettingsVisibility(bool visible)
    {
        root.Q<VisualElement>("SettingsPanelRoot").visible = visible;
    }

    public void SaveSettings()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        APIManager.APIData apiData = new APIManager.APIData();
        apiData.openAIAPIKey = root.Q<TextField>("OpenAIApiKey").text;
        apiData.openAIModel = root.Q<DropdownField>("OpenAIModelDropdown").text;
        apiData.openAIInitialPrompt = root.Q<TextField>("OpenAIInitialPrompt").text;
        apiData.APIType = root.Q<DropdownField>("OpenAIApiType").text;
        apiData.elevenLabsAPIKey = root.Q<TextField>("ElevenLabsAPIKey").text;
        apiData.elevenLabsVoiceID = root.Q<TextField>("ElevenLabsVoiceID").text;
        apiData.elevenLabsStability = (float)root.Q<Slider>("ElevenLabsStabilitySlider").value/100f;
        apiData.elevenLabsSimilarity = (float)root.Q<Slider>("ElevenLabsSimilaritySlider").value / 100f;
        APIManager.instance.apiData = apiData;

    }

    public void UpdateSettingsView(APIManager.APIData apiData)
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<TextField>("OpenAIApiKey").value = apiData.openAIAPIKey;
        root.Q<DropdownField>("OpenAIModelDropdown").value = apiData.openAIModel;
        root.Q<TextField>("OpenAIInitialPrompt").value = apiData.openAIInitialPrompt;
        root.Q<DropdownField>("OpenAIApiType").value = apiData.APIType;
        root.Q<TextField>("ElevenLabsAPIKey").value = apiData.elevenLabsAPIKey;
        root.Q<TextField>("ElevenLabsVoiceID").value = apiData.elevenLabsVoiceID;
        root.Q<Slider>("ElevenLabsStabilitySlider").value = apiData.elevenLabsStability*100f;
        root.Q<Slider>("ElevenLabsSimilaritySlider").value = apiData.elevenLabsSimilarity*100f;
    }

    public void AddMessage(string message, bool isFromUser)
    {
        if(chatHistory == null)
        {
            chatHistory = new List<ChatElement>();
        }

        chatHistory.Add(new ChatElement(isFromUser, message));
        RefreshView();
    }

    private void RefreshView()
    {
        waifuChatHistory.Clear();
        var waifuMessageVisualTree = Resources.Load<VisualTreeAsset>("UI/Templates/WaifuMessage");
        var userMessageVisualTree = Resources.Load<VisualTreeAsset>("UI/Templates/UserMessage");
        foreach (var chatElement in chatHistory)
        {
            TemplateContainer templateContainer;
            if (chatElement.fromUser)
                templateContainer = userMessageVisualTree.Instantiate();
            else
                templateContainer = waifuMessageVisualTree.Instantiate();
            templateContainer.Q<Label>("ChatText").text = chatElement.message;
            waifuChatHistory.Add(templateContainer);
        }
    }
}
