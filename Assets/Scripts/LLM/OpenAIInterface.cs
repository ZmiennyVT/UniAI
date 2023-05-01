using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Threading.Tasks;

public class OpenAIInterface : LLMInterface
{
    private OpenAIClient client;
    private List<Message> chatHistory;
    private Model openAIModelType;

    private void Awake()
    {
        onLLMResponse = new UnityEngine.Events.UnityEvent<string>();
    }

    public override void Init()
    {
        base.Init();
        this.Init("");
    }

    public void Init(string key, string initPrompt="")
    {
        if(key == "")
        {
            Debug.LogError("API Key is required to use OpenAIInterface");
            return;
        }

        client = new OpenAIClient(key);
        chatHistory = new List<Message>();
        

        if(initPrompt != "")
        {
            chatHistory.Add(new Message(Role.System, initPrompt));
        }

        openAIModelType = Model.GPT3_5_Turbo; //move to init

    }

    public override async Task SendRequest(string request)
    {
        Debug.Log("LLM Request Accepted: " + request);
        chatHistory.Add(new Message(Role.User, request));
        var chatRequest = new ChatRequest(chatHistory, openAIModelType);
        var result = await client.ChatEndpoint.GetCompletionAsync(chatRequest);
        Debug.Log("I got the response: " + result.FirstChoice.ToString());
        chatHistory.Add(new Message(Role.Assistant, result.FirstChoice.ToString()));
        await this.ResponseHandle(result.FirstChoice.ToString());
    }

    public override Task ResponseHandle(string response)
    {
        onLLMResponse.Invoke(response);
        return Task.CompletedTask;
    }
}
