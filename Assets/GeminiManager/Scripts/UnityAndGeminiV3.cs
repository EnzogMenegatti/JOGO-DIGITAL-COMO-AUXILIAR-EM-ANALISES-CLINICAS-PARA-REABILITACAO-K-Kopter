using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using System.IO; 
using System;

#region Serialized Classes
[System.Serializable]
public class UnityAndGeminiKey
{
    public string key;
}

[System.Serializable]
public class InlineData
{
    public string mimeType;
    public string data;
}

[System.Serializable]
public class TextPart
{
    public string text;
}

[System.Serializable]
public class ImagePart
{
    public string text;
    public InlineData inlineData;
}

[System.Serializable]
public class TextContent
{
    public string role;
    public TextPart[] parts;
}

[System.Serializable]
public class TextCandidate
{
    public TextContent content;
}

[System.Serializable]
public class TextResponse
{
    public TextCandidate[] candidates;
}

[System.Serializable]
public class ImageContent
{
    public string role;
    public ImagePart[] parts;
}

[System.Serializable]
public class ImageCandidate
{
    public ImageContent content;
}

[System.Serializable]
public class ImageResponse
{
    public ImageCandidate[] candidates;
}

[System.Serializable]
public class ChatRequest
{
    public TextContent[] contents;
    public TextContent system_instruction;
}
#endregion

public class UnityAndGeminiV3 : MonoBehaviour
{
    [Header("JSON API Configuration")]
    public TextAsset jsonApi;

    private const string modelName = "gemini-3.5-flash";
    private string apiKey = ""; 

    [Header("ChatBot Function")]
    public TMP_InputField inputField;
    public TMP_Text uiText;
    public string botInstructions;
    private List<TextContent> chatHistory = new List<TextContent>();

    [Header("Prompt Function")]
    public string prompt = "";

    // Protection against HTTP 429 (Too Many Requests)
    private bool isRequesting = false;

    void Start()
    {
        if (jsonApi == null) { Debug.LogError("Please assign the JSON API TextAsset!"); return; }
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;
        if (prompt != "") { StartCoroutine(SendPromptRequestToGemini(prompt)); }
    }

    // Helper to queue requests
    private IEnumerator WaitUntilReady()
    {
        while (isRequesting)
        {
            yield return new WaitForSeconds(0.1f); 
        }
    }

    private IEnumerator SendPromptRequestToGemini(string promptText)
    {
        yield return StartCoroutine(WaitUntilReady());
        isRequesting = true;
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";
        // Fixed: Removed the extra curly braces that were wrapping the prompt string
        string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"" + promptText + "\"}]}]}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError("Error: " + www.error + "\nResponse: " + www.downloadHandler.text);
            } else {
                Debug.Log("Request complete!");
                TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
                if (response.candidates != null && response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                {
                    string text = response.candidates[0].content.parts[0].text;
                    Debug.Log("Prompt Output: " + text);
                }
                else { Debug.Log("No text found."); }
            }
        }
        isRequesting = false;
    }

    public void SendChat()
    {
        string userMessage = inputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            StartCoroutine(SendChatRequestToGemini(userMessage));
            inputField.text = ""; // Optional: Clear field after sending
        }
    }

    private IEnumerator SendChatRequestToGemini(string newMessage)
    {
        yield return StartCoroutine(WaitUntilReady());
        isRequesting = true;

        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";     
        TextContent userContent = new TextContent {
            role = "user",
            parts = new TextPart[] { new TextPart { text = newMessage } }
        };

        TextContent instruction = new TextContent {
            parts = new TextPart[] { new TextPart { text = botInstructions } }
        }; 

        chatHistory.Add(userContent);

        ChatRequest chatRequest = new ChatRequest { 
            contents = chatHistory.ToArray(), 
            system_instruction = instruction 
        };

        string jsonData = JsonUtility.ToJson(chatRequest);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError("Chat Error: " + www.error);
            } else {
                Debug.Log("Request complete!");
                TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
                if (response.candidates != null && response.candidates.Length > 0)
                {
                    string reply = response.candidates[0].content.parts[0].text;
                    uiText.text = reply;
                    
                    chatHistory.Add(new TextContent {
                        role = "model",
                        parts = new TextPart[] { new TextPart { text = reply } }
                    });
                    Debug.Log("AI: " + reply);
                }
             }
        }  
        isRequesting = false;
    }
}