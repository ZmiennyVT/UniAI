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

    public LLMInterface llmInterface;
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
            llmInterface.SendToLLM(root.Q<TextField>("UserInput").text);
        };

        waifuChatHistory.Clear();
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
