using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class CollisionData
{
    public string TaskID;
    public string object1;
    public string object2;
    public string timestamp;
    public string groupName; // Include group name
}

[System.Serializable]
public class GroupData
{
    public string groupName;
}

public class SavingRocketData : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000";
    private string groupName = ""; // Store group name

    void Start()
    {
        StartCoroutine(GetGroupNameRequest(baseUrl + "/group"));
    }

    IEnumerator GetGroupNameRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error retrieving group name: " + request.error);
            }
            else
            {
                string jsonResponse = "{\"data\":" + request.downloadHandler.text + "}"; // Wrap for parsing
                GroupData[] groups = JsonUtility.FromJson<GroupWrapper>(jsonResponse).data;

                if (groups.Length > 0)
                {
                    groupName = groups[0].groupName; // Extract first group's name
                    Debug.Log("Group Name Retrieved: " + groupName);
                }
                else
                {
                    Debug.LogWarning("No group data found in response!");
                }
            }
        }
    }

    [System.Serializable]
    private class GroupWrapper
    {
        public GroupData[] data;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("LandingPadWorst") || col.gameObject.CompareTag("LandingPadMid") || col.gameObject.CompareTag("LandingPadBest"))
        {
            CollisionData collisionData = new CollisionData
            {
                TaskID = "Rocket Landing task",
                object1 = gameObject.name,
                object2 = col.gameObject.tag,
                timestamp = System.DateTime.Now.ToString(),
                groupName = groupName // Store retrieved group name
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