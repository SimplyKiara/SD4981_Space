using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public GameObject mapPanel;
    public GameObject mapButton;  // button for opening map

    private ClientConnection clientConnection;
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; //"http://localhost:3000/TaskDone";
    private bool dataLoaded = false;
    public float checkInterval = 5f; // Time interval for checking (in seconds)


    void OnEnable()
    {
        clientConnection = GameObject.Find("Client Manager").GetComponent<ClientConnection>();
        StartCoroutine(CheckForExpedition());
    }

    IEnumerator CheckForExpedition()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/TaskDone"))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error retrieving expedition data: {request.error}");
                }
                else
                {
                    string jsonResponse = $"{{\"tdone\":{request.downloadHandler.text}}}"; // Wrap JSON in an object
                    Debug.Log($"Formatted JSON: {jsonResponse}");

                    try
                    {
                        // Deserialize the formatted JSON correctly
                        TaskDataList taskDataList = JsonUtility.FromJson<TaskDataList>(jsonResponse);

                        if (taskDataList?.tdone?.Length > 0)
                        {
                            foreach (TaskDoneData taskDone in taskDataList.tdone)
                            {
                                // Only retreive expedition of same group
                                if (taskDone.TaskID == "Expedition" && taskDone.group == clientConnection.groupName)
                                {
                                    Debug.Log("Received JSON: " + request.downloadHandler.text);

                                    dataLoaded = true;
                                    mapButton.SetActive(true);
                                    Debug.Log("Expedition done, showing map.");
                                    break;   // Exit loop once a match is found
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("No matching expedition task found. Checking again...");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error parsing JSON: {ex.Message}");
                    }
                }
            }
            yield return new WaitForSeconds(checkInterval); // Wait before rechecking
        }
    }


    public void OpenMap()
    {
        mapPanel.SetActive(true);
    }

    public void CloseMap()
    {
        mapPanel.SetActive(false);
    }
}
