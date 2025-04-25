using SocketIOClient;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientConnection : MonoBehaviour
{
    public Button getTaskBtn;
    private SocketIOUnity socket;

    [System.Serializable]
    public class GroupUpdate
    {
        public string events;
        public GroupData data;
    }

    [System.Serializable]
    public class GroupData
    {
        public string groupName;
        public string session;
    }

    [System.Serializable]
    public class Task
    {
        public string title;
    }

    private string baseUrl = "http://localhost:3000"; // "http://10.11.36.4:3000";

    async void Start()
    {
        var uri = new System.Uri("http://localhost:3000");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new System.Collections.Generic.Dictionary<string, string>
            {
                { "token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connection open!");
        };

        socket.OnError += (sender, e) =>
        {
            Debug.LogError("Error! " + e);
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Connection closed!");
        };

        socket.On("groupUpdated", response =>
        {
            var message = response.ToString();
            Debug.Log("OnMessage! " + message);

            GroupUpdate groupUpdate = JsonUtility.FromJson<GroupUpdate>(message);
            Debug.Log("Group updated: " + groupUpdate.data.groupName + ", Session: " + groupUpdate.data.session);
        });

        await socket.ConnectAsync();
    }

    public void Awake()
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
                    if (!string.IsNullOrEmpty(taskResponse.title))
                    {
                        Debug.Log($"Task Name: {taskResponse.title}");

                        // Pass taskName to ClientManager
                        ClientManager clientManager = FindObjectOfType<ClientManager>();
                        if (clientManager != null)
                        {
                            clientManager.ActivateButton(taskResponse.title);
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
