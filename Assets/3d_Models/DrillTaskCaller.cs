using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DrillTaskCaller : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000";
    private bool dataLoaded = false;
    public float checkInterval = 5f; // Time interval for checking

    private Vector3[] directions =
    {
        new Vector3(5, 7, -5),
        new Vector3(5, 9, -5),
        new Vector3(-5, 7, -5)
    };

    void Start()
    {
        StartCoroutine(CheckForMiningData());
    }

    IEnumerator CheckForMiningData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error retrieving mining data: " + request.error);
                }
                else
                {
                    string jsonResponse = "{:" + request.downloadHandler.text + "}"; // Wrap JSON in an object
                    Debug.Log("Formatted JSON: " + jsonResponse);

                    // Deserialize as a wrapper class
                    RocketDataList rocketDataList = JsonUtility.FromJson<RocketDataList>(jsonResponse);

                    

                    if (!dataLoaded)
                    {
                        Debug.LogWarning("No matching object found. Checking again...");
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval); // Wait before rechecking
        }
    }
}
