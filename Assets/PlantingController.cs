using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SocketIOClient;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Net.Sockets;
using static SocketIOManager;

public class PlantingController : MonoBehaviour
{
    public Text ResourcesText;
    public Text AnnounceText;
    public int groupId = 0;

    private string baseUrl = "http://";
    private bool called = false;

    private SocketIOUnity socket;

    [System.Serializable]
    public class GroupUpdate
    {
        public string events;
        public GroupData data;
    }

    [System.Serializable]
    public class GroupData
    {
        public string groupName;
        public string session;
    }

    async void Start()
    {
        var uri = new System.Uri("http://localhost:3000");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new System.Collections.Generic.Dictionary<string, string>
            {
                { "token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connection open!");
        };

        socket.OnError += (sender, e) =>
        {
            Debug.LogError("Error! " + e);
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Connection closed!");
        };

        socket.On("groupUpdated", response =>
        {
            var message = response.ToString();
            Debug.Log("OnMessage! " + message);

            GroupUpdate groupUpdate = JsonUtility.FromJson<GroupUpdate>(message);
            Debug.Log("Group updated: " + groupUpdate.data.groupName + ", Session: " + groupUpdate.data.session);
        });

        await socket.ConnectAsync();
    }

    private void Awake()
    {
        
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
            //SendMessageToServer();
        }
    }

    private async void SendMessageToServer(string message)
    {
        if (socket.Connected)
        {
            await socket.EmitAsync("message", message);
            Debug.Log("Message sent: " + message);
        }
    }

    private async void OnApplicationQuit()
    {
        await socket.DisconnectAsync();
    }
}

