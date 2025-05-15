using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SocketIOClient;
using System.Linq;
using TMPro;

public class PlayerInfoManager : MonoBehaviour
{
    public GameObject playerInfoCardPrefab; // Reference to the Player Info Card prefab
    public Transform playerInfoGrid; // Reference to the Grid Layout Group parent
    public Transform winnerGrid;
    public GameObject startBtn;
    public GameObject endBtn;
    public GameObject reloadBtn;
    public GameObject sceneBtn;
    public Button connectBtn;
    public GameObject endPanel; // ending panel that show result
    public Button backToMenuBtn; // navigate back to menu
    public GameObject menuPanel;
    public GameObject playerInfoPanel;
    public TMP_InputField urlField;
    public GameObject[] scenePanels;
    private string serverUrl; // = "http://localhost:3000"; Replace with your server URL
    private SocketIOUnity socket;
    private bool isGameStarted = false;
    bool isConnected = false;
    private bool sceneStatus = false;

    [System.Serializable]
    public class PlayerData
    {
        public string _id;
        public string groupName;
        public string progress;
        public int __v;

        // Helper method to get progress as integer
        public int GetProgressValue()
        {
            int progressValue;
            return int.TryParse(progress, out progressValue) ? progressValue : 0;
        }
    }

    private List<PlayerData> players = new List<PlayerData>();
    private bool playerUpdated = false;
    public void UpdateURL()
    {
        serverUrl = urlField.text;
        Debug.Log("" + serverUrl);
    }
    void Start()
    {
        menuPanel.SetActive(true);
        playerInfoPanel.SetActive(false);
        endPanel.SetActive(false);
        reloadBtn.SetActive(false);
        // connectBtn.onClick.AddListener(OnConnection);
        connectBtn.onClick.AddListener(OnConnectClicked);
        backToMenuBtn.onClick.AddListener(OnBackToMenuClicked);
        serverUrl = "https://spaceexpeditionserver.onrender.com"; // "http://localhost:3000";

    }
    public async void OnConnectClicked()
    {
        menuPanel.SetActive(false);
        playerInfoPanel.SetActive(true);
        reloadBtn.SetActive(true);
        var uri = new System.Uri(serverUrl);
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
            isConnected = true;
        };

        socket.OnError += (sender, e) =>
        {
            Debug.LogError("Error! " + e);
            BackToConnectionMenu();
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Connection closed!");
            isConnected = false;
        };

        socket.On("groupUpdated", response =>
        {
            var message = response.ToString();
            Debug.Log("OnMessage! " + message);
            string json = message.Trim('[', ']');
            PlayerData groupUpdate = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Group updated: " + groupUpdate.groupName + ", Session: " + groupUpdate.progress);
            playerUpdated = true;
        });

        await socket.ConnectAsync();
    }

    void Update()
    {
        if (playerUpdated && isConnected)
        {
            Refresh();
            playerUpdated = false;
        }
        if (players != null && players.Count == 3 && isConnected && !isGameStarted)
        {
            startBtn.SetActive(true);
        }
        else if (isGameStarted)
        {
            endBtn.SetActive(true);
            sceneBtn.SetActive(true);
        }
    }

    public void Refresh()
    {
        Debug.Log("Refresh called");
        if (playerInfoGrid.childCount != 0)
        {
            Debug.Log("child found");
            foreach (Transform child in playerInfoGrid)
            {
                Destroy(child.gameObject);
                Debug.Log("child removed");
            }
            players.Clear();
        }
        StartCoroutine(GetPlayerData());
        Debug.Log("data retrieved");
    }

    public void ChangeScenePanel()
    {
        if (scenePanels.Count() == 2)
        {
            if (!sceneStatus && !scenePanels[0].activeInHierarchy)
            {
                scenePanels[0].SetActive(true);   // Base To Site
                sceneStatus = true;
            }
            else if (sceneStatus && !scenePanels[1].activeInHierarchy)
            {
                scenePanels[1].SetActive(true);   // Site To Base
                sceneStatus = false;
            }
            else
            {
                Debug.Log("Cannot open panel, maybe opened already?");
            }
        }
        else
        {
            Debug.LogError("Scene-chaning panels not set up correctly");
        }
    }

    private void BackToConnectionMenu()
    {
        // Reopen the connection panel
        menuPanel.SetActive(true);
        playerInfoPanel.SetActive(false);
        endPanel.SetActive(false);
        reloadBtn.SetActive(false);
        urlField.text = "";
        serverUrl = "https://spaceexpeditionserver.onrender.com"; // "http://localhost:3000";
    }

    IEnumerator GetPlayerData()
    {
        Debug.Log("GetPlayerData");
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl + "/group"))
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
                            players.Add(player);
                            Debug.Log($"[json] {player.groupName} {player.progress}");
                            AddPlayerInfoCard(player);
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

    void AddPlayerInfoCard(PlayerData playerData)
    {
        GameObject newCard = Instantiate(playerInfoCardPrefab, playerInfoGrid);
        Debug.Log($"[info card] {playerData.groupName} {playerData.progress}");
        newCard.transform.Find("AvatarArea").transform.Find("Identity").GetComponent<TMP_Text>().text = playerData.groupName;
        newCard.transform.Find("AvatarArea").transform.Find("TasksDone").GetComponent<TMP_Text>().text = playerData.progress;
        Debug.Log($"[identity: ] {newCard.transform.Find("AvatarArea").transform.Find("Identity").GetComponent<TMP_Text>().text}");
        Debug.Log($"[progress: ] {newCard.transform.Find("AvatarArea").transform.Find("TasksDone").GetComponent<TMP_Text>().text}");
    }

    void OnApplicationQuit()
    {
        StartCoroutine(DeleteGroupData());
        StartCoroutine(DeleteTasksData());
        socket.DisconnectAsync();
        isConnected = false;
    }

    IEnumerator DeleteGroupData()
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(serverUrl + "/group"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error deleting group data: " + request.error);
            }
            else
            {
                Debug.Log("Group data deleted successfully");
            }
        }
    }

    IEnumerator DeleteTasksData()
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(serverUrl + "/allTasks"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error deleting group data: " + request.error);
            }
            else
            {
                Debug.Log("Tasks data deleted successfully");
            }
        }
    }

    // socket send scene changing
    public void SendChatMessage(string content, string user)
    {
        var messageData = new { content, user };
        socket.Emit("send_message", messageData);
        Debug.Log($"Message sent: {content}");
    }

    public void StartGame()
    {
        if (socket.Connected)
        {
            socket.Emit("start game");
            Debug.Log("Start game message emitted");
            startBtn.SetActive(false);
            isGameStarted = true;
        }
        else
        {
            Debug.LogError("Socket not connected");
        }
    }
    public void EndGame()
    {
        if (socket.Connected)
        {
            endBtn.SetActive(false);
            reloadBtn.SetActive(false);
            playerInfoPanel.SetActive(false);
            endPanel.SetActive(true);
            DisplayWinner();
            isGameStarted = false;
        }
        else
        {
            Debug.LogError("Socket not connected");
        }
    }

    public void DisplayWinner()
    {
        if (players == null || players.Count == 0) return;

        // Clear existing winner display
        foreach (Transform child in winnerGrid)
        {
            Destroy(child.gameObject);
        }

        // Find player with highest progress
        PlayerData winner = players.OrderByDescending(p => p.GetProgressValue()).First();

        // Emit winner information through socket
        socket.Emit("game winner", winner.groupName);
        Debug.Log($"Winner emitted: {winner.groupName} with tasks done {winner.progress}");

        // Create winner card
        GameObject winnerCard = Instantiate(playerInfoCardPrefab, winnerGrid);
        winnerCard.transform.Find("AvatarArea").transform.Find("Identity").GetComponent<TMP_Text>().text = winner.groupName;
        winnerCard.transform.Find("AvatarArea").transform.Find("TasksDone").GetComponent<TMP_Text>().text = winner.progress;
    }

    public void OnBackToMenuClicked()
    {
        StartCoroutine(DeleteGroupData());
        StartCoroutine(DeleteTasksData());
        players.Clear();
        foreach (Transform child in playerInfoGrid) Destroy(child.gameObject);
        foreach (Transform child in winnerGrid) Destroy(child.gameObject);
        // Disconnect the socket
        if (socket.Connected)
        {
            socket.Emit("end game");
            Debug.Log("End game message emitted");
            socket.DisconnectAsync();
            Debug.Log("Socket disconnected");
            isConnected = false;
        }
        BackToConnectionMenu();
    }
}
