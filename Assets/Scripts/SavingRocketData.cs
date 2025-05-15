using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class TaskData
{
    public string _id;
    public string title;
    public string group;
}

[System.Serializable]
public class TaskWrapper
{
    public TaskData[] data;
}

[System.Serializable]
public class CollisionData
{
    public string TaskID;
    public string object1;
    public string object2;
    public string timestamp;
    public string groupName; // Include group name
}

public class SavingRocketData : MonoBehaviour
{
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; // Adjust if needed
    private string groupName = ""; // Store group name

    void Start()
    {
        StartCoroutine(GetLatestTaskData(baseUrl + "/tasks"));
    }

    IEnumerator GetLatestTaskData(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error retrieving task data: " + request.error);
            }
            else
            {
                string jsonResponse = "{\"data\":" + request.downloadHandler.text + "}"; // Wrap for parsing
                TaskData[] tasks = JsonUtility.FromJson<TaskWrapper>(jsonResponse).data;

                if (tasks.Length > 0)
                {
                    groupName = tasks[tasks.Length - 1].group; // Get the most recent task's group
                    Debug.Log("Latest Group Retrieved: " + groupName);
                }
                else
                {
                    Debug.LogWarning("No task data found in response!");
                }
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            Debug.LogWarning("Group name not yet retrieved. Collision data won't be sent yet.");
            return;
        }

        if (col.gameObject.CompareTag("LandingPadWorst") || col.gameObject.CompareTag("LandingPadMid") || col.gameObject.CompareTag("LandingPadBest"))
        {
            CollisionData collisionData = new CollisionData
            {
                TaskID = "Rocket Landing task",
                object1 = gameObject.name,
                object2 = col.gameObject.tag,
                timestamp = System.DateTime.Now.ToString(),
                groupName = groupName // Use retrieved group name
            };

            string json = JsonUtility.ToJson(collisionData);
            StartCoroutine(PostCollisionDataRequest(baseUrl + "/Rocket", json));

            Debug.Log("Rocket landing data saved! " + json);
        }
    }

    IEnumerator PostCollisionDataRequest(string uri, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending collision data to server...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending collision data: " + request.error);
            }
            else
            {
                Debug.Log("Server Response: " + request.downloadHandler.text);
            }
        }
    }
}