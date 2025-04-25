using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ProgressManager : MonoBehaviour
{
    public enum GameState
    {
        Lobby = 0,
        MoonHillBackground = 1,
        DigCaveMinigame = 2,
        RocketLandingMinigame = 3,
        BuildBaseStage1 = 4,
        PlantingMinigame = 5,
        IceMiningMinigame = 6,
        HarvestMinigame = 7,
        BuildBaseStage2 = 8,
        GameEnd = 9
    }
    public GameState currentState;
    public Slider progressBar;

    void Start()
    {
        currentState = GameState.Lobby;
        UpdateGameState();
    }

    public void UpdateGameState()
    {
        switch (currentState)
        {
            case GameState.Lobby:
                // Initialize lobby
                break;
            case GameState.MoonHillBackground:
                // Show moon hill background
                break;
            case GameState.DigCaveMinigame:
                // Start dig cave minigame
                break;
            case GameState.RocketLandingMinigame:
                // Start rocket landing minigame
                break;
            case GameState.BuildBaseStage1:
                // Build first stage of the base
                break;
            case GameState.PlantingMinigame:
                // Start planting minigame
                break;
            case GameState.IceMiningMinigame:
                // Start ice mining minigame
                break;
            case GameState.HarvestMinigame:
                // Start harvest minigame
                break;
            case GameState.BuildBaseStage2:
                // Build second stage of the base
                break;
            case GameState.GameEnd:
                // End game
                break;
        }
        UpdateProgressBar();
        SaveGameState();
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;
        UpdateGameState();
    }

    void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.value = (int)currentState / (float)GameState.GameEnd;
        }
    }
    void SaveGameState()
    {
        StartCoroutine(SaveGameStateCoroutine(currentState.ToString()));
    }
    [System.Serializable]
    public class State
    {
        public string gameState;
    }

    IEnumerator SaveGameStateCoroutine(string state)
    {
        State gameState = new State { gameState = state };
        string jsonData = JsonUtility.ToJson(gameState);
        string url = "http://localhost:3000/gameState";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}
