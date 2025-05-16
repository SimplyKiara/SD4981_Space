using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using SocketIOClient;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class TaskData
{
    public string _id;
    public string title;
    public string group;
}

[System.Serializable]
public class TaskWrapper
{
    public TaskData[] data;
}

[System.Serializable]
public class CollisionData
{
    public string TaskID;
    public string object1;
    public string object2;
    public string timestamp;
    public string groupName; // Include group name
}

public class SavingRocketData : MonoBehaviour
{
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; // Adjust if needed
    public string groupName = ""; // Store group name
    private SocketIOUnity socket;
    void Start()
    {
        // StartCoroutine(GetLatestTaskData(baseUrl + "/tasks"));
        OnStart();
    }
    void Update()
    {
        if (groupName == "")
        {
            ClientConnection clientConnection = GameObject.Find("Client Manager").GetComponent<ClientConnection>();
            groupName = clientConnection.groupName;
            // groupName = "Group " + Random.Range(1, 4);
            /* GameObject groupNameObj = GameObject.Find("GroupName");
            if (groupNameObj != null)
            {
                groupName = groupNameObj.GetComponent<TMP_Text>().text;
            } */

        }
    }
    async void OnStart()
    {
        var uri = new System.Uri(baseUrl);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
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

        await socket.ConnectAsync();
    }
    /* IEnumerator GetLatestTaskData(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error retrieving task data: " + request.error);
            }
            else
            {
                string jsonResponse = "{\"data\":" + request.downloadHandler.text + "}"; // Wrap for parsing
                TaskData[] tasks = JsonUtility.FromJson<TaskWrapper>(jsonResponse).data;

                if (tasks.Length > 0)
                {
                    groupName = tasks[tasks.Length - 1].group; // Get the most recent task's group
                    Debug.Log("Latest Group Retrieved: " + groupName);
                }
                else
                {
                    Debug.LogWarning("No task data found in response!");
                }
            }
        }
    } */

    void OnCollisionEnter(Collision col)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            Debug.LogWarning("Group name not yet retrieved. Collision data won't be sent yet.");
            return;
        }

        if (col.gameObject.CompareTag("LandingPadWorst") || col.gameObject.CompareTag("LandingPadMid") || col.gameObject.CompareTag("LandingPadBest"))
        {
            CollisionData collisionData = new CollisionData
            {
                TaskID = "Rocket Landing task",
                object1 = gameObject.name,
                object2 = col.gameObject.tag,
                timestamp = System.DateTime.Now.ToString(),
                groupName = groupName // Use retrieved group name
            };

            string json = JsonUtility.ToJson(collisionData);
            StartCoroutine(PostCollisionDataRequest(baseUrl + "/Rocket", json));
            socket.Emit("updateRocket");

            Debug.Log("Rocket landing data saved! " + json);
        }
    }

    IEnumerator PostCollisionDataRequest(string uri, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending collision data to server...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending collision data: " + request.error);
            }
            else
            {
                Debug.Log("Server Response: " + request.downloadHandler.text);
            }
        }
    }
}