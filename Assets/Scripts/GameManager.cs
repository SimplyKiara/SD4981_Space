using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance { get; private set; }

    public string FileName;  // file name for data saving
    private FileDataHandler dataHandler;

    public List<GameObject> solarPanels;
    public GameObject GreenHouse;
    public GameObject UpgradedBase;
    public Text ironOreText;
    public Text rocksText;
    public Text waterText;
    public Text groupNameText; // UI element for displaying group name
    public GameData gameData;

    [NonSerialized] public int ironOre;
    [NonSerialized] public int rocks;
    [NonSerialized] public float water;
    [NonSerialized] public float waterCap;

    [SerializeField] private string groupName = "Fetching..."; // Group name displayed in Inspector
    [SerializeField] public string RocketLanded = "Fetching...";
    private string baseUrl = "https://spaceexpeditionserver.onrender.com"; //"http://localhost:3000";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure persistence across scene changes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    private void Start()
    {
        string dataPath = Application.persistentDataPath; // Use platform-independent path
        dataHandler = new FileDataHandler(Application.persistentDataPath, FileName); // No encryption parameter needed
        LoadGame();
    }

    public void LoadData(GameData data)
    {
        if (data != null)
        {
            gameData = data;
            ironOre = data.ironOre;
            rocks = data.rocks;
            water = data.water;
            waterCap = GameData.maxWater;
            UpdateUI();
            Debug.Log($"Loaded {FileName}: Iron = {ironOre}, Rocks = {rocks}, Water = {water}/{waterCap}");
        }
        else
        {
            Debug.Log($"No saved data found for {FileName}, initializing new game.");
            gameData = new GameData();
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data == null)
        {
            Debug.LogError("SaveData failed: GameData object is null!");
            return;
        }

        data.ironOre = ironOre;
        data.rocks = rocks;
        data.water = water;
        string fullPath = Path.Combine(Application.persistentDataPath, FileName);
        Debug.Log($"Saved {FileName}, File Path = {fullPath}: Iron = {gameData.ironOre}, Rocks = {gameData.rocks}, Water = {gameData.water}/{waterCap}");
    }

    public void LoadGame()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, FileName);

        if (!File.Exists(fullPath))
        {
            Debug.Log($"Save file {FileName} not found. Creating new game data.");
            gameData = new GameData();
            dataHandler.Save(gameData);
            return;
        }

        try
        {
            string rawJson = File.ReadAllText(fullPath);

            if (rawJson.StartsWith("GAME_SAVE_FORMAT"))
            {
                rawJson = rawJson.Substring("GAME_SAVE_FORMAT".Length).Trim();
            }

            GameData loadedData = JsonUtility.FromJson<GameData>(rawJson);

            if (loadedData != null)
            {
                gameData = loadedData;
                LoadData(gameData);
                Debug.Log($"Game Loaded ({FileName}): Iron = {gameData.ironOre}, Rocks = {gameData.rocks}, Water = {gameData.water}/{GameData.maxWater}");
            }
            else
            {
                Debug.LogWarning("Failed to parse JSON. Creating new game data.");
                gameData = new GameData();
                dataHandler.Save(gameData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data from {FileName}: {e.Message}");
            gameData = new GameData();
            dataHandler.Save(gameData);
        }
    }


    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning($"SaveGame failed for {FileName}: gameData is null! Initializing new GameData.");
            gameData = new GameData();
        }

        SaveData(ref gameData); // Ensure in-memory updates are applied
        dataHandler.Save(gameData); // Save updated data

        Debug.Log($"Game Saved! ({FileName})");
    }


    [System.Serializable]
    private class GroupWrapper
    {
        public List<GroupData> data;
    }

    [System.Serializable]
    private class GroupData
    {
        public string groupName;
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
                string jsonResponse = "{\"data\":" + request.downloadHandler.text + "}"; // Wrapping response for parsing
                GroupWrapper groupWrapper = JsonUtility.FromJson<GroupWrapper>(jsonResponse);

                if (groupWrapper.data.Count > 0)
                {
                    groupName = groupWrapper.data[0].groupName; // Extract first group's name
                    Debug.Log("Group Name Retrieved: " + groupName);

                    // Update UI text
                    if (groupNameText != null)
                        groupNameText.text = "Group Name: " + groupName;
                }
                else
                {
                    Debug.LogWarning("No group data found in response!");
                }
            }
        }
    }

    public void BuildSolarPanels()
    {
        if (solarPanels != null)
        {
            if (ironOre >= 8 && rocks >= 8)
            {
                bool panelActivated = false;

                foreach (GameObject panel in solarPanels)
                {
                    if (!panel.gameObject.activeSelf)
                    {
                        panel.gameObject.SetActive(true);
                        panelActivated = true;
                        break; // Exit the loop after activating one panel
                    }
                }

                if (panelActivated)
                {
                    AddCollectedIron(-8);
                    AddCollectedRocks(-8);
                    Debug.Log("Build successful! Iron: " + ironOre + ", Rocks: " + rocks);

                    // Save updated data
                    DataPersistenceManager.instance.SaveGame();
                }
                else
                {
                    Debug.Log("Solar Panels: Maximum reached");
                }
            }
            else
            {
                Debug.Log("Solar Panels: Not enough resources");
            }
        }
    }

    public void UpgradeBase()
    {
        if (UpgradedBase != null)
        {
            if (ironOre >= 25 && rocks >= 25)
            {
                if (!UpgradedBase.activeSelf)
                {
                    AddCollectedIron(-25);
                    AddCollectedRocks(-25);
                    UpgradedBase.SetActive(true);
                }
                else
                {
                    Debug.Log("Base upgrade: Already built");
                }
            }
            else
            {
                Debug.Log("Base upgrade: Not enough resources");
            }
        }
    }

    public void BuildGreenhouse()
    {
        if (GreenHouse != null)
        {
            if (ironOre >= 20 && rocks >= 20 && water >= 5)
            {
                if (!GreenHouse.activeSelf)
                {
                    AddCollectedIron(-20);
                    AddCollectedRocks(-20);
                    ChangeCollectedWater(-5);
                    GreenHouse.SetActive(true);
                }
                else
                {
                    Debug.Log("Greenhouse: Already built");
                }
            }
            else
            {
                Debug.Log("Greenhouse: Not enough resources");
            }
        }
    }

    public void AddCollectedIron(int value)
    {
        ironOre += value;
        UpdateUI();
    }

    public void AddCollectedRocks(int value)
    {
        rocks += value;
        UpdateUI();
    }

    public void ChangeCollectedWater(float value)
    {
        if ((water + value) < waterCap)
        {
            water += value;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if ((ironOreText != null) && (rocksText != null) && (waterText != null))
        {
            ironOreText.text = "Iron ore: " + ironOre;
            rocksText.text = "Lunar rocks: " + rocks;
            waterText.text = $"Water: {water}/{waterCap}";
        }

        if (groupNameText != null)
        {
            groupNameText.text = "Group Name: " + groupName; // Show group name in UI
        }
    }
}