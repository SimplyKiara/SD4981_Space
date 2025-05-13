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
    public GameObject rocketObject; // Attach the GameObject in the Inspector
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; // "http://localhost:3000/Rocket";
    private bool dataLoaded = false;
    public GameManager gameManager;
    public float checkInterval = 5f; // Time interval for checking (in seconds)

    void Start()
    {
        // Start with the object inactive
        if (rocketObject != null) rocketObject.SetActive(false);
        StartCoroutine(CheckForRocketData());
    }

    IEnumerator CheckForRocketData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
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
                    string jsonResponse = "{\"rockets\":" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    Debug.Log("Formatted JSON: " + jsonResponse);

                    // Deserialize JSON
                    RocketDataList rocketDataList = JsonUtility.FromJson<RocketDataList>(jsonResponse);

                    foreach (RocketData rocket in rocketDataList.rockets)
                    {
                        if (rocket.groupName == "Group 1") // Check if groupName matches
                        {
                            GameObject targetObject = GameObject.FindWithTag("gp1"); // Find GameObject with tag "gp1"
                            if (targetObject != null)
                            {
                                targetObject.name = rocket.object2; // Assign object2 value
                                gameManager.RocketLanded = rocket.object2;
                                Debug.Log($"Assigned '{rocket.object2}' to GameObject with tag 'gp1'.");
                            }
                            else
                            {
                                Debug.LogWarning("No GameObject found with tag 'gp1'.");
                            }
                        }

                        if (rocket.object2 == "LandingPadMid")
                        {
                            ActivateRocket(rocket, 10);
                            break;
                        }
                        else if (rocket.object2 == "LandingPadBest")
                        {
                            ActivateRocket(rocket, 20);
                            break;
                        }
                        else if (rocket.object2 == "LandingPadWorst")
                        {
                            ActivateRocket(rocket, 5);
                            break;
                        }
                    }

                    if (!dataLoaded)
                    {
                        Debug.LogWarning("No matching object2 found. Checking again...");
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval); // Wait before rechecking
        }
    }

    void ActivateRocket(RocketData rocket, int resourceAmount)
    {
        dataLoaded = true;
        if (rocketObject != null) rocketObject.SetActive(true);
        Debug.Log($"Matching object2 found: {rocket.object2}. Activating rocketObject.");

        // Add resources based on landing quality
        gameManager.AddCollectedIron(resourceAmount);
        gameManager.AddCollectedRocks(resourceAmount);
        gameManager.ChangeCollectedWater(resourceAmount);
    }
}