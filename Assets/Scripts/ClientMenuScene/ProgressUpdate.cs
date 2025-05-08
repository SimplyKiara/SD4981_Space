using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressUpdate : MonoBehaviour
{
    public ClientConnection clientConnection;
    bool isTaskStart;
    // Start is called before the first frame update
    void Start()
    {
        clientConnection = FindAnyObjectByType<ClientConnection>();
        isTaskStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "ClientMenu" && !isTaskStart)
        {
            isTaskStart = true;
        }
        else if (SceneManager.GetActiveScene().name == "ClientMenu" && isTaskStart)
        {
            clientConnection.UpdateProgress();
            isTaskStart = false;
        }

    }
}
