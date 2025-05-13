using SocketIOClient;
using System;
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
    public GameObject selectPanel;
    public TMP_InputField urlField;
    public string groupName;
    public string progress;
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
    public class AvaliableTask
    {
        public string title;
        public string group;
    }

    public string baseUrl; //  = "https://space-expedition-server.vercel.app" "http://localhost:3000" "http://10.11.36.4:3000";

    public void UpdateURL()
    {
        baseUrl = urlField.text;
        Debug.Log("" + baseUrl);
    }

    void Start()
    {
        menuPanel.SetActive(true);
        waitPanel.SetActive(false);
        selectPanel.SetActive(false);
        // connectBtn.onClick.AddListener(OnConnection);
        connectBtn.onClick.AddListener(OnConnectClicked);
        progress = "0";
        baseUrl = "https://spaceexpeditionserver.onrender.com"; // "http://localhost:3000";

    }

    public void OnConnection()
    {
        StartCoroutine(GetRequest(baseUrl));
        StartCoroutine(GetGroupData());
    }
    private async void OnConnectClicked()
    {
        menuPanel.SetActive(false);
        waitPanel.SetActive(true);
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
            isConnected = true;
        };

        socket.OnError += (sender, e) =>
        {
            Debug.LogError("Error! " + e);
            HandleConnectionError();
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
            isConnected = false;
            isReady = false;
        });

        await socket.ConnectAsync();
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
            selectPanel.SetActive(true);
        }
    }

    string GenGroupName()
    {
        return $"Group {groupList.Count + 1}";
    }

    public void OnGetTaskClicked()
    {
        // StartCoroutine(GetLatestTaskRequest(baseUrl + "/tasks"));
        StartCoroutine(GetTasksRequest(baseUrl + "/tasks"));
    }

    private void HandleConnectionError()
    {
        // Reopen the connection panel
        menuPanel.SetActive(true);
        waitPanel.SetActive(false);
        selectPanel.SetActive(false);
        urlField.text = "";
        baseUrl = "https://spaceexpeditionserver.onrender.com"; // "http://localhost:3000";
    }

    public void UpdateProgress()
    {
        try
        {
            int preProgress = Convert.ToInt32(progress);
            Console.WriteLine("Converted the {0} value '{1}' to the {2} value {3}.", progress.GetType().Name, progress, progress.GetType().Name, preProgress);
            int newProgress = preProgress + 1;
            if (isConnected)
            {
                var updateData = new
                {
                    groupName = groupName,
                    updateData = new { progress = newProgress.ToString() }
                };
                socket.EmitAsync("updateGroup", updateData);
                Debug.Log("Update group event emitted with progress: " + progress);
            }
            progress = newProgress.ToString();
        }
        catch (OverflowException)
        {
            Console.WriteLine("{0} is outside the range of the Int32 type.", progress);
        }
        catch (FormatException)
        {
            Console.WriteLine("The {0} value '{1}' is not in a recognizable format.", progress.GetType().Name, progress);
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                HandleConnectionError();
            }
            else
            {
                // messageText.text = request.downloadHandler.text;
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    /* IEnumerator GetLatestTaskRequest(string uri)
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
                        // ClientManager clientManager = FindObjectOfType<ClientManager>();
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
    } */

    IEnumerator GetTasksRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                HandleConnectionError();
            }
            else
            {
                string jsonString = request.downloadHandler.text;
                Debug.Log("Response JSON: " + jsonString);
                if (jsonString != "[]")
                {
                    try
                    {
                        List<string> jsonObjects = jsonString.Split(new string[] { "},{" }, System.StringSplitOptions.None).Select(p => p.Trim('[', ']')).ToList();
                        foreach (string jsonObject in jsonObjects)
                        {
                            string formattedJson = "{" + jsonObject.Trim('{', '}') + "}";
                            Debug.Log("[task data] " + formattedJson);
                            AvaliableTask avaliableTask = JsonUtility.FromJson<AvaliableTask>(formattedJson);
                            Debug.Log($"[task json] {avaliableTask.title} {avaliableTask.group}");
                            if (avaliableTask.group == groupName)
                            {
                                // Pass taskName to ClientManager
                                ClientManager clientManager = GetComponent<ClientManager>();
                                // ClientManager clientManager = FindObjectOfType<ClientManager>();
                                if (clientManager != null)
                                {
                                    clientManager.ActivateButton(avaliableTask.title);
                                }
                                else
                                {
                                    Debug.LogError("ClientManager not found!");
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"JSON Parsing Error: {ex.Message}");
                        HandleConnectionError();
                    }
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
                HandleConnectionError();
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
                        HandleConnectionError();
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
                HandleConnectionError();
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}

