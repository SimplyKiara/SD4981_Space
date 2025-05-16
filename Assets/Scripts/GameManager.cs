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
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
        LoadGame();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        if (gameData == null)
        {
            gameData = new GameData();
            Debug.Log("Initialized new GameData.");
        }

        LoadData(gameData);
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("SaveGame failed: gameData is null! Initializing new GameData.");
            gameData = new GameData();
        }

        SaveData(ref gameData);
        dataHandler.Save(gameData);
    }

    public void LoadData(GameData data)
    {
        if (data == null)
        {
            Debug.LogWarning("LoadData error: data is null!");
            return;
        }

        ironOre = data.ironOre;
        rocks = data.rocks;
        water = data.water;
        waterCap = GameData.maxWater;
        UpdateUI();
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
        Debug.Log($"Saving: Iron = {data.ironOre}, Rocks = {data.rocks}, Water = {data.water}/{waterCap}");
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
        if (ironOreText && rocksText && waterText)
        {
            ironOreText.text = $"Iron ore: {ironOre}";
            rocksText.text = $"Lunar rocks: {rocks}";
            waterText.text = $"Water: {water}/{waterCap}";
        }

        if (groupNameText != null)
        {
            groupNameText.text = "Group Name: " + groupName; // Show group name in UI
        }
    }
}