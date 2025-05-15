using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SocketIOClient;
using System;

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

    public void DeleteTask(string taskName, string group)
    {
        // TaskGenerator taskSelector = new TaskGenerator();
        // string randomTask = taskSelector.GetRandomTask();

        StartCoroutine(DeleteTaskRequest(baseUrl + "/tasks", taskName, group));
    }

    IEnumerator DeleteTaskRequest(string uri, string title, string group)
    {
        // Create a new UnityWebRequest for the DELETE method
        UnityWebRequest request = new UnityWebRequest(uri, "DELETE");

        // Create a JSON object with the title and group
        string jsonData = JsonUtility.ToJson(new { title = title, group = group });

        // Convert the JSON object to a byte array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Set the request body
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the request headers
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Task deleted successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error deleting task: " + request.error);
        }
    }

    public void ChangeScene(string content, string user)
    {
        StartCoroutine(PostSceneChangeRequest(baseUrl + "/messages", content, user));
    }

    [System.Serializable]
    public class Scene
    {
        public string content;
        public string user;
    }

    IEnumerator PostSceneChangeRequest(string uri, string content, string user)
    {
        Scene scene = new Scene { content = content, user = user };
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
