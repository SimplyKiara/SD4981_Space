using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class IceData
{
    public string TaskID;
    public string groupName;
}

public class DrillTaskCaller : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/";
    private bool dataLoaded = false;

    private Vector3[] directions =
    {
        new Vector3(3, 5, -3),
        new Vector3(-5, 5, -3),
        new Vector3(-3, 5, -6),
    };

    public float checkInterval = 5f; // Time interval for checking (in seconds)
    public GameObject IceModel;

    void Start()
    {
        StartCoroutine(CheckForIceData());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            ActivateDrill();
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
                    string jsonResponse = "{" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    Debug.Log("Formatted JSON: " + jsonResponse);

                    // Deserialize as a wrapper class
                    IceData iceData = JsonUtility.FromJson<IceData>(jsonResponse);
                    if (iceData != null )
                    {
                        ActivateDrill();
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
