using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class TaskDoneData
{
    public string TaskID;
    public string timestamp;
    public string group;
}

// Wrapper class for deserializing an array
[System.Serializable]
public class TaskDataList
{
    public TaskDoneData[] tdone;
}

public class ResourceReceiver : MonoBehaviour
{
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; //"http://localhost:3000/TaskDone";
    private bool dataLoaded = false;
    public GameManager gameManager;
    public float checkInterval = 5f; // Time interval for checking (in seconds)

    string Name;
    private string gpName;

    private void Start()
    {
        StartCoroutine(CheckForIceData());

        Name = gameManager.name.Substring(0, 3);

        if (Name == "Gp1")
        {
            gpName = "Group 1";
        }
        else if (Name == "Gp2")
        {
            gpName = "Group 2";
        }
        else if (Name == "Gp3")
        {
            gpName = "Group 3";
        }
        else
        {
            Debug.LogError("Group name not obtainable!");
        }
    }

    IEnumerator CheckForIceData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            GameObject groupDrill = GameObject.Find(Name + "_DrillStructure");

            if (groupDrill == null)
            {
                Debug.LogError("Group / Drill not found!");
                break;
            }

            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/TaskDone"))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error retrieving ice mining data: " + request.error);
                }
                else
                {
                    string jsonResponse = "{\"tdone\":" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    Debug.Log("Formatted JSON: " + jsonResponse);

                    // Directly deserialize into TaskDataList
                    TaskDataList taskDataList = JsonUtility.FromJson<TaskDataList>(request.downloadHandler.text);

                    if (taskDataList != null && taskDataList.tdone.Length > 0)
                    {
                        foreach (TaskDoneData taskDone in taskDataList.tdone)
                        {
                            if (taskDone.group == gpName)
                            {
                                DrillTaskCaller drillScript = groupDrill.GetComponent<DrillTaskCaller>();
                                if (drillScript != null)
                                {
                                    drillScript.ActivateDrill();
                                }
                                else
                                {
                                    Debug.LogError("DrillTaskCaller script not found on object");
                                }
                                break; // Exit loop once a match is found
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No matching group found. Checking again...");
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval); // Wait before rechecking
        }
    }
}
