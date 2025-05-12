using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEditor.U2D.Aseprite;
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

public class DrillTaskCaller : MonoBehaviour
{
    public GameObject popupPrefab; // Assign the UI popup prefab
    public GameManager gameManager;

    private string gpName;
    private GameObject currentPopup;

    private string baseUrl = "http://localhost:3000/TaskDone";
    private bool dataLoaded = false;

    private Vector3[] directions =
    {
        new Vector3(3, 5, -3),
        new Vector3(-5, 5, -3),
        new Vector3(-3, 5, -6),
    };

    public float checkInterval = 5f; // Time interval for checking (in seconds)
    public GameObject IceModel;

    private void Start()
    {
        StartCoroutine(CheckForIceData());

        // Find all child objects with LongPressGesture
        foreach (Transform child in transform)
        {
            LongPressGesture longPressGesture = child.GetComponent<LongPressGesture>();

            if (longPressGesture != null)
            {
                longPressGesture.StateChanged += longPressedHandler;
                Debug.Log($"Subscribed to {child.name}'s LongPressGesture");
            }
        }

        string name = gameManager.name.Substring(0, 3);

        if (name == "Gp1")
        {
            gpName = "Group 1";
        }
        else if (name == "Gp2")
        {
            gpName = "Group 2";
        }
        else if (name == "Gp3")
        {
            gpName = "Group 3";
        }
        else
        {
            Debug.LogError("Group name not obtainable!");
        }
    }

    private void longPressedHandler(object sender, GestureStateChangeEventArgs e)
    {
        Debug.Log("Long press detected on child!");

        if (e.State == Gesture.GestureState.Recognized)
        {
            ShowUIPopup();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            ActivateDrill();
        }
    }

    private void ShowUIPopup()
    {
        if (popupPrefab != null)
        {
            if (currentPopup == null)
            {
                currentPopup.SetActive(true);
                Debug.Log($"Supplies UI instantiated");
            }
            else
            {
                Debug.Log("Popup already exists, not creating a new one.");
            }
        }
        else
        {
            Debug.LogError("Popup prefab or target canvas is missing!");
        }
    }

    IEnumerator CheckForIceData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl))
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
                                ActivateDrill();
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

    void ActivateDrill()
    {
        dataLoaded = true;
        Debug.Log($"Data received, spawning ice.");

        foreach (Vector3 spawnPosition in directions)
        {
            GameObject ice = Instantiate(IceModel, transform.position + spawnPosition, Quaternion.identity);
            ice.name = "IceChunk";
            ice.transform.parent = transform.parent;
            ice.SetActive(true);
        }
    }

}
