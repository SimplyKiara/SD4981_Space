using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ServerConnection : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text messageText;
    private string baseUrl = "http://localhost:3000";
    [System.Serializable]
    public class Message
    {
        public string content;
    }

    public void Start()
    {
        StartCoroutine(GetRequest(baseUrl));
    }
    public void OnGetBtnClicked()
    {
        StartCoroutine(GetAllMessagesRequest(baseUrl + "/messages"));
    }
    public void OnPostBtnClicked()
    {
        StartCoroutine(PostMessageRequest(baseUrl + "/messages", inputField.text));
    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                messageText.text = request.downloadHandler.text;
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
    IEnumerator GetAllMessagesRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                messageText.text = request.downloadHandler.text;
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
    IEnumerator PostMessageRequest(string uri, string content)
    {
        Message message = new Message { content = content };
        string jsonData = JsonUtility.ToJson(message);

        Debug.Log("JSON Data: " + jsonData); // Debug log to check JSON data

        using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}
