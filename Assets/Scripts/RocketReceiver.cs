using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SocketIOClient;
using System.Linq;
using System.Collections;
using System;

[System.Serializable]
public class RocketData
{
    public string TaskID;
    public string object1;
    public string object2;
    public string timestamp;
    public string groupName;
}

[System.Serializable]
public class RocketDataList
{
    public RocketData[] rockets;
}

public class RocketReceiver : MonoBehaviour
{
    public GameObject rocketObject1;
    public GameObject rocketObject2;
    public GameObject rocketObject3;
    public GameObject reminder1;
    public GameObject reminder2;
    public GameObject reminder3;

    private string baseUrl = "https://spaceexpeditionserver.onrender.com";
    private Dictionary<string, string> lastTimestamps = new Dictionary<string, string>();
    private List<RocketData> rocketDataList = new List<RocketData>();
    private SocketIOUnity socket;
    private bool isRocketUpdated = false;
    public GameManager gameManager1;
    public GameManager gameManager2;
    public GameManager gameManager3;
    private DateTime latestTimestamp = DateTime.MinValue;

    void Start()
    {
        lastTimestamps["Group 1"] = "";
        lastTimestamps["Group 2"] = "";
        lastTimestamps["Group 3"] = "";
        SetRocketState(false);
        OnStart();
    }

    async void OnStart()
    {
        var uri = new System.Uri(baseUrl);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.On("rocketUpdate", res =>
        {
            isRocketUpdated = true;
        });

        await socket.ConnectAsync();
    }

    void Update()
    {
        if (isRocketUpdated)
        {
            StartCoroutine(FetchRocketData());
            isRocketUpdated = false;
        }
    }

    IEnumerator FetchRocketData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/Rocket"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error retrieving rocket data: " + request.error);
            }
            else
            {
                string jsonResponse = "{\"rockets\":" + request.downloadHandler.text + "}";
                RocketDataList newRocketDataList = JsonUtility.FromJson<RocketDataList>(jsonResponse);

                if (newRocketDataList.rockets.Length > 0)
                {
                    rocketDataList.Clear();
                    rocketDataList = newRocketDataList.rockets.ToList();

                    RocketData latestRocket = rocketDataList.OrderByDescending(r => DateTime.Parse(r.timestamp)).FirstOrDefault();

                    if (latestRocket != null)
                    {
                        DateTime latestTime = DateTime.Parse(latestRocket.timestamp);
                        if (latestTime > latestTimestamp)
                        {
                            latestTimestamp = latestTime;
                            lastTimestamps[latestRocket.groupName] = latestRocket.timestamp;
                            ActivateRocket(GetGameManagerByGroupName(latestRocket.groupName), GetRocketObjectByGroupName(latestRocket.groupName), latestRocket);
                        }
                    }
                }
            }
        }
    }

    GameManager GetGameManagerByGroupName(string groupName)
    {
        switch (groupName.Trim())
        {
            case "Group 1": return gameManager1;
            case "Group 2": return gameManager2;
            case "Group 3": return gameManager3;
            default:
                Debug.LogWarning($"Unknown groupName: {groupName}");
                return null;
        }
    }

    GameObject GetRocketObjectByGroupName(string groupName)
    {
        switch (groupName.Trim())
        {
            case "Group 1": return rocketObject1;
            case "Group 2": return rocketObject2;
            case "Group 3": return rocketObject3;
            default:
                Debug.LogWarning($"Unknown groupName: {groupName}");
                return null;
        }
    }

    void ActivateRocket(GameManager targetManager, GameObject targetRocket, RocketData rocket)
    {
        if (targetManager == null || targetRocket == null) return;

        targetRocket.SetActive(true);
        Debug.Log($"New rocket data detected for {rocket.groupName}. Activating corresponding rocket.");

        int resourceAmount = GetResourceAmount(rocket.object2);
        targetManager.AddCollectedIron(resourceAmount);
        targetManager.AddCollectedRocks(resourceAmount);
        targetManager.ChangeCollectedWater(resourceAmount);
    }

    int GetResourceAmount(string objectName)
    {
        switch (objectName)
        {
            case "LandingPadBest": return 20;
            case "LandingPadMid": return 10;
            case "LandingPadWorst": return 5;
            default: return 0;
        }
    }

    void SetRocketState(bool state)
    {
        if (rocketObject1 != null) rocketObject1.SetActive(state);
        if (rocketObject2 != null) rocketObject2.SetActive(state);
        if (rocketObject3 != null) rocketObject3.SetActive(state);
        if (rocketObject1.activeSelf) reminder1.SetActive(true);
        if (rocketObject2.activeSelf) reminder2.SetActive(true);
        if (rocketObject3.activeSelf) reminder3.SetActive(true);
    }
}