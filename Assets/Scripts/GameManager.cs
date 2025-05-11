using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance { get; private set; }
    public List<SolarPanel> solarPanels;
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
    private string baseUrl = "http://localhost:3000";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager found in the scene.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(GetGroupNameRequest(baseUrl + "/group"));
        UpdateUI();
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

    public void LoadData(GameData data)
    {
        gameData = data;

        ironOre = data.ironOre;
        rocks = data.rocks;
        water = data.water;
        waterCap = GameData.maxWater;
        UpdateUI();
    }


    public void SaveData(ref GameData data)
    {
        data.ironOre = ironOre;
        data.rocks = rocks;
        data.water = water;
    }

    public void BuildSolarPanels()
    {
        if (ironOre >= 8 && rocks >= 8)
        {
            int activatedCount = 0;

            foreach (SolarPanel panel in solarPanels)
            {
                if (!panel.gameObject.activeSelf)
                {
                    panel.gameObject.SetActive(true);
                    activatedCount++;
                    if (activatedCount >= 1) break;
                }
            }

            if (activatedCount >= 1)
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
        if (ironOre >= 20 && rocks >= 20 && water >= 8)
        {
            if (!GreenHouse.activeSelf)
            {
                AddCollectedIron(-20);
                AddCollectedRocks(-20);
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