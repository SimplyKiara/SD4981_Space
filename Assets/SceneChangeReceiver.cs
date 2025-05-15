using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneMessage
{
    public string content;
    public string user;
}

// Wrapper class for deserializing an array
[System.Serializable]
public class SceneMessageList
{
    public SceneMessage[] scMessage;
}

public class SceneChangeReceiver : MonoBehaviour
{
    public GameObject SceneChangingCanvas;

    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; //"http://localhost:3000/TaskDone";
    private bool dataLoaded = false;
    public float checkInterval = 5f; // Time interval for checking (in seconds)
    private string currentSceneName;
    private string nextSceneName;

    private void Start()
    {
        SceneChangingCanvas.SetActive(false);
        currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "MainWallScene")
        {
            nextSceneName = "ResourcesScene";
        }
        else if (currentSceneName == "ResourcesScene")
        {
            nextSceneName = "MainWallScene";
        }
        else
        {
            Debug.LogError("Scene names not loaded correctly.");
        }
    }

    IEnumerator CheckForIceData()
    {
        while (!dataLoaded) // Continuously check until valid data is found
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/messages"))
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
                    SceneMessageList sceneMessageList = JsonUtility.FromJson<SceneMessageList>(request.downloadHandler.text);

                    if (sceneMessageList != null && sceneMessageList.scMessage.Length > 0)
                    {
                        foreach (SceneMessage sceneMessage in sceneMessageList.scMessage)
                        {
                            if (sceneMessage.content == nextSceneName)
                            {
                                if (sceneMessage.user == "Teacher")
                                {
                                    if (SceneChangingCanvas != null)
                                    {
                                        SceneChangingCanvas.SetActive(true);
                                    }
                                    Invoke("ChangeToAnotherScene", 5f);
                                }
                                else
                                {
                                    Debug.Log("Scene changing: not enough access");
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Scene changing: already changed scene / not receiving scene names correctly");
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

    void ChangeToAnotherScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
