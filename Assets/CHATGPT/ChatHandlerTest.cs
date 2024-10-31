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
    private string openAIKey = "";
    private string openAIUrl = "https://api.openai.com/v1/chat/completions";

    [Header("UI Elements")]
    public TMP_InputField userInputField;
    public Button submitButton;
    public TMP_Text responseText;

    [Header("Polly Manager")]
    public PollyManagerTest pollyManager; 

    private Animator animator;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitButtonClicked);
        animator = GetComponent<Animator>();
    }

    // Check if the gaze is off screen..will need to be updated with webgazer

    // get responses
    private void OnSubmitButtonClicked()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            userInputField.text = "";
            HandleResponseLogic(userMessage); 
        }
    }

    public string GetLatestChatResponse()
{
    return responseText.text; 
}
    // get responses from openAI
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

    // Handles response logic based on detected user feedback
    private void HandleResponseLogic(string userMessage)
    {
        bool needsChatGPTResponse = true;

        if (userMessage.Contains("I'm fine") || userMessage.Contains("No, I do not need help focusing"))
        {
            pollyManager.SynthesizeTextToSpeech("You got this!");
            needsChatGPTResponse = false;
        }
        else if (userMessage.Contains("Yes, I need help focusing"))
        {
            pollyManager.SynthesizeTextToSpeech("Enter 1 for a quick stretch to help refocus in interaction input.");
            needsChatGPTResponse = false;
        } 
        else if (userMessage.Contains("1"))
        {
            pollyManager.SynthesizeTextToSpeech("Let's stretch! Follow along.");
            StartStretchAnimation();
            needsChatGPTResponse = false;
        }

        // if none of the predefined reponses were used, proceed to get responses from openAI
        if (needsChatGPTResponse)
        {
            StartCoroutine(GetChatGPTResponse(userMessage));
        }
    }

    // Starts a stretching animation on the virtual human
    private void StartStretchAnimation()
    {
        animator.SetTrigger("Stretching");
    }
} 

