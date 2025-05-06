using SocketIOClient;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ClientConnection : MonoBehaviour
{

    public Button connectBtn;
    public TMP_Text groupDisplay;
    public GameObject menuPanel;
    public GameObject waitPanel;
    public GameObject gamePanel;
    public string groupName;
    public string progress = "0";
    private SocketIOUnity socket;
    List<PlayerData> groupList = new List<PlayerData>();
    bool isConnected = false;
    bool isReady = false;
    bool isEnded = false;

    [System.Serializable]
    public class PlayerData
    {
        public string groupName;
        public string progress;
    }

    [System.Serializable]
    public class GroupList
    {
        public PlayerData[] groups;
    }

    [System.Serializable]
    public class avaliableTask
    {
        public string title;
    }

    private string baseUrl = "http://localhost:3000"; // "http://10.11.36.4:3000";


    void Start()
    {
        menuPanel.SetActive(true);
        waitPanel.SetActive(false);
        gamePanel.SetActive(false);
        connectBtn.onClick.AddListener(OnConnectClicked);
        isConnected = false;
        isReady = false;
        isEnded = false;
    }

    private async void OnConnectClicked()
    {
        menuPanel.SetActive(false);
        waitPanel.SetActive(true);
        StartCoroutine(GetGroupData());

        var uri = new System.Uri(baseUrl);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new System.Collections.Generic.Dictionary<string, string>
            {
                { "token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += async (sender, e) =>
        {
            Debug.Log("Connection open!");
            isConnected = true;
        };

        socket.OnError += (sender, e) =>
        {
            Debug.LogError("Error! " + e);
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Connection closed!");
            isConnected = false;
        };

        /* socket.On("groupUpdated", response =>
        {
            var message = response.ToString();
            Debug.Log("OnMessage! " + message);
            string json = message.Trim('[', ']');
            PlayerData groupUpdate = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Group updated: " + groupUpdate.groupName + ", Session: " + groupUpdate.progress);
        }); */

        socket.On("gameStarted", response =>
        {
            Debug.Log("start game");
            isReady = true;
        });

        socket.On("gameEnded", response =>
        {
            Debug.Log("Game ended message received!");
            isEnded = true;
        });

        await socket.ConnectAsync();
    }

    public void Awake()
    {
        StartCoroutine(GetRequest(baseUrl));
    }
    public void Update()
    {
        if (isConnected && groupName == "")
        {
            groupName = GenGroupName();
            StartCoroutine(PostGroupData(groupName, progress));
            groupDisplay.text = groupName;
        }

        if (isReady && waitPanel.activeInHierarchy)
        {
            waitPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
    }

    string GenGroupName()
    {
        return $"Group {groupList.Count + 1}";
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

    IEnumerator GetGroupData()
    {
        Debug.Log("GetPlayerData");
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/group"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string jsonString = request.downloadHandler.text;
                Debug.Log("Response: " + jsonString);
                if (jsonString != "[]")
                {
                    try
                    {
                        List<string> jsonObjects = jsonString.Split(new string[] { "},{" }, System.StringSplitOptions.None).Select(p => p.Trim('[', ']')).ToList();
                        foreach (string jsonObject in jsonObjects)
                        {
                            string formattedJson = "{" + jsonObject.Trim('{', '}') + "}";
                            Debug.Log(formattedJson);
                            PlayerData player = JsonUtility.FromJson<PlayerData>(formattedJson);
                            Debug.Log($"[json] {player.groupName} {player.progress}");
                            groupList.Add(player);
                        }

                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("JSON Parsing Error: " + e.Message);
                    }
                }
            }
        }
    }
    IEnumerator PostGroupData(string groupName, string progress)
    {
        PlayerData playerData = new PlayerData { groupName = groupName, progress = progress };
        string jsonData = JsonUtility.ToJson(playerData);

        Debug.Log("JSON Data: " + jsonData); // Debug log to check JSON data

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/group", "POST"))
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

