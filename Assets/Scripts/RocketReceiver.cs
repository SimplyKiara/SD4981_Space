using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class RocketData
{
    public string TaskID;
    public string object1;
    public string object2;
    public string timestamp;
    public string groupName; // Added groupName field
}

// Wrapper class for deserializing an array
[System.Serializable]
public class RocketDataList
{
    public RocketData[] rockets;
}

public class RocketReceiver : MonoBehaviour
{
    public GameObject rocketObject1; // Corresponds to gameManager1
    public GameObject rocketObject2; // Corresponds to gameManager2
    public GameObject rocketObject3; // Corresponds to gameManager3
    public GameObject reminder1;
    public GameObject reminder2;
    public GameObject reminder3;

    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; // "http://localhost:3000/Rocket";
    private string lastTimestamp1 = ""; // Store last timestamp for Group 1
    private string lastTimestamp2 = ""; // Store last timestamp for Group 2
    private string lastTimestamp3 = ""; // Store last timestamp for Group 3

    private bool firstCheckDone = false; // Prevent activation on first database check

    // Game Managers for each group
    public GameManager gameManager1;
    public GameManager gameManager2;
    public GameManager gameManager3;

    public float checkInterval = 5f; // Time interval for checking (in seconds)

    void Start()
    {
        // Ensure all rocket objects start inactive
        SetRocketState(false);
        reminder1.SetActive(false);
        reminder2.SetActive(false);
        reminder3.SetActive(false);
        StartCoroutine(CheckForRocketData());
    }

    IEnumerator CheckForRocketData()
    {
        while (true) // Continuously check for updates
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
                    RocketDataList rocketDataList = JsonUtility.FromJson<RocketDataList>(jsonResponse);

                    foreach (RocketData rocket in rocketDataList.rockets)
                    {
                        // Determine which GameManager this rocket belongs to
                        if (rocket.groupName == "Group 1" && rocket.timestamp != lastTimestamp1)
                        {
                            lastTimestamp1 = rocket.timestamp;
                            ActivateRocket(gameManager1, rocketObject1, rocket);
                        }
                        else if (rocket.groupName == "Group 2" && rocket.timestamp != lastTimestamp2)
                        {
                            lastTimestamp2 = rocket.timestamp;
                            ActivateRocket(gameManager2, rocketObject2, rocket);
                        }
                        else if (rocket.groupName == "Group 3" && rocket.timestamp != lastTimestamp3)
                        {
                            lastTimestamp3 = rocket.timestamp;
                            ActivateRocket(gameManager3, rocketObject3, rocket);
                        }
                    }

                    if (!firstCheckDone)
                    {
                        Debug.Log("First database check complete, skipping activation.");
                        firstCheckDone = true; // Prevent activation during the initial check
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval); // Keep checking
        }
    }

    void ActivateRocket(GameManager targetManager, GameObject targetRocket, RocketData rocket)
    {
        if (targetManager == null || targetRocket == null) return;

        if (firstCheckDone) // Ensure activation only happens on new database updates
        {
            targetRocket.SetActive(true);
            Debug.Log($"New rocket data detected for {rocket.groupName}: {rocket.object2}. Activating corresponding rocket.");
            Debug.Log("Target manager is " + targetManager);
            // Add resources based on landing quality
            int resourceAmount = GetResourceAmount(rocket.object2);
            Debug.Log("resouce amount is: " + resourceAmount);
            targetManager.AddCollectedIron(resourceAmount);
            targetManager.AddCollectedRocks(resourceAmount);
            targetManager.ChangeCollectedWater(resourceAmount);
        }
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