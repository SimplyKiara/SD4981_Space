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
    public float checkInterval = 5f; // Time interval for checking (in seconds)

    private string gpName;

    private void OnEnable()
    {
        StartCoroutine(CheckForIceData());
    }

    IEnumerator CheckForIceData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/TaskDone"))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error retrieving ice mining data: " + request.error);
                }
                else
                {
                    // Ensure the JSON response is not empty
                    if (string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.LogWarning("Received empty response, checking again...");
                        yield return new WaitForSeconds(checkInterval);
                        continue;
                    }

                    Debug.Log("Received JSON: " + request.downloadHandler.text);

                    // Ensure JSON is properly structured before deserialization
                    string jsonResponse = "{\"tdone\":" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    TaskDataList taskDataList = JsonUtility.FromJson<TaskDataList>(jsonResponse);

                    if (taskDataList != null && taskDataList.tdone.Length > 0)
                    {
                        foreach (TaskDoneData taskDone in taskDataList.tdone)
                        {
                            string Name = taskDone.group;
                            string gpName = "";

                            switch (Name)
                            {
                                case "Group 1":
                                    gpName = "Gp1";
                                    break;
                                case "Group 2":
                                    gpName = "Gp2";
                                    break;
                                case "Group 3":
                                    gpName = "Gp3";
                                    break;
                                default:
                                    Debug.LogError($"Group name '{Name}' not recognized!");
                                    continue; // Skip to the next task
                            }

                            GameObject groupDrill = GameObject.Find(gpName + "_DrillStructure");
                            if (groupDrill != null)
                            {
                                DrillTaskCaller drillScript = groupDrill.GetComponent<DrillTaskCaller>();
                                if (drillScript != null)
                                {
                                    drillScript.ActivateDrill();
                                    dataLoaded = true;
                                    Debug.Log($"Activated drill for {gpName}");
                                }
                                else
                                {
                                    Debug.LogError("DrillTaskCaller script not found on object");
                                }
                            }
                            else
                            {
                                Debug.LogError($"Drill structure for {gpName} not found!");
                            }
                            break;   // Exit loop once a match is found
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
