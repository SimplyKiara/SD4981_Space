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
    public GameObject startBtn;
    public GameObject endBtn;
    private string serverUrl = "http://localhost:3000"; // Replace with your server URL
    private SocketIOUnity socket;
    private bool isGameStarted = false;

    [System.Serializable]
    public class PlayerData
    {
        public string _id;
        public string groupName;
        public string progress;
        public int __v;

        // Other player fields if needed
    }

    private List<PlayerData> players = new List<PlayerData>();
    private bool playerUpdated = false;

    async void Start()
    {
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
            string json = message.Trim('[', ']');
            PlayerData groupUpdate = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Group updated: " + groupUpdate.groupName + ", Session: " + groupUpdate.progress);
            playerUpdated = true;
        });

        await socket.ConnectAsync();
    }

    void Update()
    {
        if (playerUpdated)
        {
            Refresh();
            playerUpdated = false;
        }
        if (players != null && players.Count == 3 && !isGameStarted)
        {
            startBtn.SetActive(true);
        }
        else if (isGameStarted)
        {
            endBtn.SetActive(true);
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
        newCard.transform.Find("AvatarArea").transform.Find("Progress").GetComponent<TMP_Text>().text = playerData.progress;
        Debug.Log($"[identity: ] {newCard.transform.Find("AvatarArea").transform.Find("Identity").GetComponent<TMP_Text>().text}");
        Debug.Log($"[progress: ] {newCard.transform.Find("AvatarArea").transform.Find("Progress").GetComponent<TMP_Text>().text}");
    }

    void OnApplicationQuit()
    {
        StartCoroutine(DeleteGroupData());
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
            socket.Emit("end game");
            Debug.Log("End game message emitted");
            StartCoroutine(DeleteGroupData());
            endBtn.SetActive(false);
            isGameStarted = false;
        }
        else
        {
            Debug.LogError("Socket not connected");
        }
    }
}
