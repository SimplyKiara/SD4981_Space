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
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.Log("Raw JSON Response: " + request.downloadHandler.text);
                    }

                    // Ensure JSON is properly structured before deserialization
                    string jsonResponse = "{\"tdone\":" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    TaskDataList taskDataList = JsonUtility.FromJson<TaskDataList>(jsonResponse);

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
                                dataLoaded = true; // Prevents continuous unnecessary checks
                                break;   // Exit loop once a match is found
                            }
                            else
                            {
                                Debug.LogError($"Greenhouse for {Name} not found!");
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
