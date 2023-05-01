using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;
    public APIData apiData;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
            apiData = new APIData();
        }
    }

    [System.Serializable]
    public class APIData{
        //OpenAI
        [Header("OpenAI")]
        public string openAIAPIKey = "sk-";
        public string openAIModel = "GPT3.5Turbo"; // ["GPT3.5", "GPT3.5Turbo", "GPT4.0"]
        public string openAIInitialPrompt = "I want you to act like anime girl";
        public string APIType = "official"; // ["official", "unofficial"]

        //ElevenLabs
        [Header("ElevenLabs")]
        public string elevenLabsAPIKey = "sk-";
        public string elevenLabsVoiceID = "";
        public float elevenLabsStability = 0.5f;
        public float elevenLabsSimilarity = 0.5f;
    }

    // TODO Do it better and more secure

    public bool LoadAPIData()
    {
        APIManager.APIData loadedApiData;
        try
        {
            loadedApiData = APIManager.instance.LoadAPIDataFromFile("AIWaifuPassword", Application.persistentDataPath + "/AIWaifuData.json");
        }
        catch { return false; }

        if (apiData != null)
        {
            this.apiData = loadedApiData;
            return true;
        }
        return false;
    }

    public bool SaveAPIData()
    {
        try
        {
            SaveAPIDataToFile(apiData, "AIWaifuPassword", Application.persistentDataPath + "/AIWaifuData.json");
            return true;
        }
        catch { return false; }
    }

    public void SaveAPIDataToFile(APIData data, string password, string filePath)
    {
        var json = JsonConvert.SerializeObject(data);
        var encryptedData = EncryptionHelper.EncryptString(json, password);
        File.WriteAllText(filePath, encryptedData);
    }

    public APIData LoadAPIDataFromFile(string password, string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Plik nie istnieje.", filePath);
        }

        var encryptedData = File.ReadAllText(filePath);
        var json = EncryptionHelper.DecryptString(encryptedData, password);
        var data = JsonConvert.DeserializeObject<APIData>(json);

        return data;
    }

}
