using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;

public class WallConnection : MonoBehaviour
{
    private SocketIOUnity socket;
    private string baseUrl = "http://localhost:3000"; // Update with your server URL

    void Start()
    {
        InitializeSocket();
    }

    private void InitializeSocket()
    {
        var uri = new System.Uri(baseUrl);
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

        socket.On("gameStarted", response =>
        {
            Debug.Log("Game started message received!");
            HandleGameStarted();
        });

        socket.On("gameEnded", response =>
        {
            Debug.Log("Game ended message received!");
            HandleGameEnded();
        });

        socket.ConnectAsync();
    }

    private void HandleGameStarted()
    {
        // Implement your logic for when the game starts
        Debug.Log("Handling game start logic...");
    }
    private void HandleGameEnded()
    {
        // Implement your logic for when the game starts
        Debug.Log("Handling game end logic...");
    }
}
