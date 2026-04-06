using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class ManagerAPI : MonoBehaviour
{
    [SerializeField] private string prompt;
    [SerializeField] private string urlAPI;



    public void FixedUpdate() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(SendDatatoURL());
        }
}

private IEnumerator SendDatatoURL()
{
    WWWForm form = new WWWForm();
    form.AddField("parameter", prompt);
    
    // Create the request
    UnityWebRequest www = UnityWebRequest.Post(urlAPI, form);
    
    // IMPORTANT: Tell Unity to follow Google's redirects
    www.redirectLimit = 5; 

    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.Success)
    {
        // If it's successful but empty, it might be the way Google is sending the data
        string response = www.downloadHandler.text;
        
        if (string.IsNullOrEmpty(response)) {
            Debug.Log("Connected to Google, but the response was empty. Check Apps Script!");
        } else {
            Debug.Log("Gemini says: " + response);
        }
    }
    else
    {
        Debug.LogError("Web Error: " + www.error);
        Debug.LogError("Full Response: " + www.downloadHandler.text);
    }
}

}
