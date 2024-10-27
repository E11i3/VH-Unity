using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;

public class ChatHandlerTest : MonoBehaviour
{
    [Header("OpenAI Settings")]
    private string openAIKey = "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";
    private string openAIUrl = "https://api.openai.com/v1/chat/completions";

    [Header("UI Elements")]
    public TMP_InputField userInputField;
    public Button submitButton;
    public TMP_Text responseText;

    [Header("Polly Manager")]
    public PollyManagerTest pollyManager; 

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitButtonClicked);
    }

    private void OnSubmitButtonClicked()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            
            userInputField.text = "";

           
            StartCoroutine(GetChatGPTResponse(userMessage));
        }
    }

    public string GetLatestChatResponse()
{
    return responseText.text; 
}


    private IEnumerator GetChatGPTResponse(string userInput)
    {
        
        string jsonPayload = "{\"model\": \"gpt-4\", \"messages\": [{\"role\": \"user\", \"content\": \"" + userInput + "\"}]}";

       
        UnityWebRequest request = new UnityWebRequest(openAIUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            string chatResponse = ParseChatGPTResponse(responseText);
            Debug.Log("ChatGPT Response: " + chatResponse);

            
            this.responseText.text = chatResponse;

            
            if (pollyManager != null)
            {
                pollyManager.SynthesizeTextToSpeech(chatResponse); 
            }
        }
        else
        {
            
            Debug.LogError("Error: " + request.error + " - Response: " + request.downloadHandler.text);
            this.responseText.text = "Failed to get response.";
        }
    }

    private string ParseChatGPTResponse(string json)
    {
        try
        {
            JObject jsonResponse = JObject.Parse(json);
            return jsonResponse["choices"][0]["message"]["content"].ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            return "Error parsing response.";
        }
    }
}
