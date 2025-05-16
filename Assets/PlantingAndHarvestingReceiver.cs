using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PlantingAndHarvestingReceiver : MonoBehaviour
{
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; //"http://localhost:3000/TaskDone";
    private bool dataLoaded = false;
    public float checkInterval = 5f; // Time interval for checking (in seconds)

    private GameObject greenHouse;
    private HashSet<string> recordedTimestamps = new HashSet<string>(); // Store timestamps

    private void OnEnable()
    {
        StartCoroutine(CheckForHarvestData());
    }

    IEnumerator CheckForHarvestData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/TaskDone"))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error retrieving planting and harvesting data: " + request.error);
                }
                else
                {
                    // Ensure the JSON response is not empty
                    if (string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.Log("Received empty response, checking again...");
                        yield return new WaitForSeconds(checkInterval);
                        continue;
                    }

                    // Ensure JSON is properly structured before deserialization
                    string jsonResponse = "{\"tdone\":" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    TaskDataList taskDataList = JsonUtility.FromJson<TaskDataList>(jsonResponse);

                    if (taskDataList != null && taskDataList.tdone.Length > 0)
                    {
                        Debug.Log("Received JSON: " + request.downloadHandler.text);

                        foreach (TaskDoneData taskDone in taskDataList.tdone)
                        {
                            // Check if timestamp has already been recorded
                            if (recordedTimestamps.Contains(taskDone.timestamp))
                            {
                                Debug.Log($"Timestamp {taskDone.timestamp} already processed. Skipping.");
                                continue;
                            }

                            // Add timestamp to the set
                            recordedTimestamps.Add(taskDone.timestamp);

                            string Name = taskDone.group;
                            string gpName = Name.Substring(7, 1);

                            greenHouse = GameObject.Find("Green House " + gpName);
                            if (greenHouse != null)
                            {
                                Invoke("CallGHouseHarvest", 120f);
                                //dataLoaded = true; // Prevents continuous unnecessary checks
                            }
                            else
                            {
                                Debug.LogWarning($"Greenhouse for {Name} not found!");
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

    void CallGHouseHarvest()
    {
        GHouseController ghController = greenHouse.GetComponent<GHouseController>();
        if (ghController != null )
        {
            ghController.CallHarvest();
        }
    }
}
