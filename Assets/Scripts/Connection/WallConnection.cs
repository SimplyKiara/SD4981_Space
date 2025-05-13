using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SocketIOClient;

public class WallConnection : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject waitPanel;
    bool isGameStarted = false;
    bool isGameEnded = false;
    private SocketIOUnity socket;
    private string baseUrl = "https://spaceexpeditionserver.onrender.com";//"http://localhost:3000"; // Update with your server URL

    void Start()
    {
        InitializeSocket();
        startPanel.SetActive(false);
        waitPanel.SetActive(true);
    }

    private void InitializeSocket()
    {
        var uri = new System.Uri(baseUrl);
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

        socket.On("gameStarted", response =>
        {
            Debug.Log("Game started message received!");
            isGameStarted = true;
        });

        socket.On("gameEnded", response =>
        {
            Debug.Log("Game ended message received!");
            isGameEnded = true;
        });

        socket.ConnectAsync();
    }

    void Update()
    {
        if (isGameStarted && waitPanel.activeInHierarchy)
        {
            // Implement your logic for when the game starts
            Debug.Log("Handling game start logic...");
            startPanel.SetActive(true);
            waitPanel.SetActive(false);
        }
        if (isGameEnded)
        {
            {
                // Implement your logic for when the game starts
                Debug.Log("Handling game end logic...");
                isGameEnded = false;
            }
        }
    }

    public void TriggerTask(string taskName, string group)
    {
        // TaskGenerator taskSelector = new TaskGenerator();
        // string randomTask = taskSelector.GetRandomTask();

        StartCoroutine(PostTaskRequest(baseUrl + "/tasks", taskName, group));
    }

    [System.Serializable]
    public class Task
    {
        public string title;
        public string group;
    }
    IEnumerator PostTaskRequest(string uri, string title, string group)
    {
        Task task = new Task { title = title, group = group };
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

    public void ChangeScene(string sceneName, string user)
    {
        StartCoroutine(PostSceneChangeRequest(baseUrl + "/scenes", sceneName, user));
    }

    [System.Serializable]
    public class Scene
    {
        public string sceneName;
        public string user;
    }
    IEnumerator PostSceneChangeRequest(string uri, string sceneName, string user)
    {
        Scene scene = new Scene { sceneName = sceneName, user = user };
        string jsonData = JsonUtility.ToJson(scene);

        Debug.Log("Scene: JSON Data: " + jsonData); // Debug log to check JSON data

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
