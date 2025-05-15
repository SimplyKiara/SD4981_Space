using System;
using System.Collections;
using System.Collections.Generic;
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


    private void Start()
    {
        string dataPath = Application.persistentDataPath;
        dataHandler = new FileDataHandler(dataPath, FileName, false); // Each manager has its own file
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
        data.ironOre = ironOre;
        data.rocks = rocks;
        data.water = water;
        Debug.Log($"Saved {FileName}: Iron = {ironOre}, Rocks = {rocks}, Water = {water}/{waterCap}");
    }

    public void LoadGame()
    {
        // Load data from file
        GameData loadedData = dataHandler.Load();

        if (loadedData != null)
        {
            LoadData(loadedData); // Apply loaded data to current instance
            Debug.Log($"Game Loaded ({FileName}): Iron = {ironOre}, Rocks = {rocks}, Water = {water}/{waterCap}");
        }
        else
        {
            Debug.Log($"Save file {FileName} not found. Creating new game data.");
            gameData = new GameData(); // Initialize default values
        }
    }

    public void SaveGame()
    {
        // Ensure current instance updates `GameData`
        SaveData(ref gameData);

        // Save to file
        dataHandler.Save(gameData);
        Debug.Log($"Game Saved ({FileName}): Iron = {ironOre}, Rocks = {rocks}, Water = {water}/{waterCap}");
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

    public void UpgradeBase()
    {
        if (ironOre >= 25 && rocks >= 25)
        {
            if (!GreenHouse.activeSelf)
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
            Debug.Log("Greenhouse: Not enough resources");
        }
    }

    public void BuildGreenhouse()
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