using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlantingController : MonoBehaviour
{
    public Text ResourcesText;
    public Text AnnounceText;
    public int groupId = 0;

    private string baseUrl = "http://";
    private bool called = false;
    private string[] groups = new string[] { "Group1", "Group2", "Group3" };


    // Structure of json data
    public class Message
    {
        public string title = "Planting Crops";
        public string group;

        public Message(string group)
        {
            this.group = group;
        }
    }

    public void Start()
    {
        Message message = new Message(groups[groupId]);
        string json = JsonUtility.ToJson(message);
        StartCoroutine(PostRequest(baseUrl, json));
    }

    void Update()
    {
        float currentWater = GameManager.instance.water;
        float maxWater = GameManager.instance.waterCap;

        ResourcesText.text = $"Water resources: {currentWater} / {maxWater}";
    }

    public void callPlanting()
    {
        if (!called)
        {
            AnnounceText.text = "Action called! Check your tablet.";

        }
    }

    IEnumerator PostRequest(string url, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending planting data to server...");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending planting data: " + request.error);
            }
            else
            {
                Debug.Log("Server Response: " + request.downloadHandler.text);
            }
        }
    }
}

