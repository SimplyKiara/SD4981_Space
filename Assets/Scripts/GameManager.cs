using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance {  get; private set; }
    public List<SolarPanel> solarPanels;
    public GameObject GreenHouse;
    public GameObject UpgradedBase;
    public Text ironOreText;
    public Text rocksText;
    public Text waterText;
    public GameData gameData;

    [NonSerialized] public int ironOre;
    [NonSerialized] public int rocks;
    [NonSerialized] public float water;
    [NonSerialized] public float waterCap;
    public string GpName;

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
        UpdateUI();
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
        if (ironOre >= 20 && rocks >= 20)
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

    public int GetCollectedIron()
    {
        return gameData.ironOre;
    }

    public void AddCollectedIron(int value)
    {
        ironOre += value;
        //Debug.Log("No. of Iron changed by" + value);
        UpdateUI();
    }

    public int GetCollectedRocks()
    {
        return gameData.rocks;
    }

    public void AddCollectedRocks(int value)
    {
        rocks += value;
        //Debug.Log("No. of Rocks changed by" + value);
        UpdateUI();
    }

    public float GetCollectedWater()
    {
        return gameData.water;
    }

    public void ChangeCollectedWater(float value)
    {
        if ((water + value) < waterCap)
        {
            water += value;
        }
        //Debug.Log("Water volume changed by" + value);
        UpdateUI();
    }

    private void UpdateUI()
    {
        ironOreText.text = "Iron ore: " + ironOre;
        rocksText.text = "Lunar rocks: " + rocks;
        waterText.text = $"Water: {water}/{waterCap}";
    }
}
