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

    IEnumerator CheckForHarvestData()
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
                    string jsonResponse = "{\"tdone\":" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    Debug.Log("Formatted JSON: " + jsonResponse);

                    // Directly deserialize into TaskDataList
                    TaskDataList taskDataList = JsonUtility.FromJson<TaskDataList>(request.downloadHandler.text);

                    if (taskDataList != null && taskDataList.tdone.Length > 0)
                    {
                        foreach (TaskDoneData taskDone in taskDataList.tdone)
                        {
                            string Name = taskDone.group;
                            string gpName = Name.Substring(7, 1);

                            greenHouse = GameObject.Find("Green House " + gpName);
                            if (greenHouse != null)
                            {
                                Invoke("CallGHouseHarvest", 120f);
                            }
                            else
                            {
                                Debug.LogError($"Drill of {Name} not found!");
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

    void CallGHouseHarvest()
    {
        GHouseController ghController = greenHouse.GetComponent<GHouseController>();
        if (ghController != null )
        {
            ghController.CallHarvest();
        }
    }
}
