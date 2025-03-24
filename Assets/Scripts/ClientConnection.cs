using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientConnection : MonoBehaviour
{
    // public TMP_Text messageText;
    // public Button taskPostBtn;
    public Button getTaskBtn;
    private string baseUrl = "http://10.11.36.4:3000";
    [System.Serializable]
    public class Task
    {
        public string title;
    }

    public void Start()
    {
        StartCoroutine(GetRequest(baseUrl));
    }

    public void OnGetTaskClicked()
    {
        StartCoroutine(GetLatestTaskRequest(baseUrl + "/tasks"));
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
                // messageText.text = request.downloadHandler.text;
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator GetLatestTaskRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string jsonString = request.downloadHandler.text;
                Debug.Log("Response JSON: " + jsonString);

                try
                {
                    // Parse JSON into TaskResponse object
                    TaskResponse taskResponse = JsonUtility.FromJson<TaskResponse>(jsonString);

                    // Check and use the taskName
                    if (!string.IsNullOrEmpty(taskResponse.taskName))
                    {
                        Debug.Log($"Task Name: {taskResponse.taskName}");

                        // Pass taskName to ClientManager
                        ClientManager clientManager = FindObjectOfType<ClientManager>();
                        if (clientManager != null)
                        {
                            clientManager.ActivateButton(taskResponse.taskName);
                        }
                        else
                        {
                            Debug.LogError("ClientManager not found!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("taskName is null or empty.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"JSON Parsing Error: {ex.Message}");
                }
            }
        }
    }

    IEnumerator PostTaskRequest(string uri, string title)
    {
        Task task = new Task { title = title };
        string jsonData = JsonUtility.ToJson(task);

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
