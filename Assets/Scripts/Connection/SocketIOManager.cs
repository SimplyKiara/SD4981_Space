using UnityEngine;
using SocketIOClient;
using System.Threading.Tasks;

public class SocketIOManager : MonoBehaviour
{
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

    void Update()
    {
        if (socket == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToServer("hello");
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
