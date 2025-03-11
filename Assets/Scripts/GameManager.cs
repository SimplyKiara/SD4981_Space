using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance {  get; private set; }
    public List<SolarPanel> solarPanels;

    public Text ironOreText;
    public Text rocksText;
    public GameData gameData;

    private int ironOre;
    private int rocks;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q pressed");
            BuildSolarPanels();
        }
    }

    public void LoadData(GameData data)
    {
        gameData = data;

        ironOre = data.ironOre;
        rocks = data.rocks;
        UpdateUI();
    }

    public void SaveData(ref GameData data)
    {
        data.ironOre = ironOre;
        data.rocks = rocks;
    }

    private void BuildSolarPanels()
    {
        if (ironOre >= 2 && rocks >= 1)
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
                AddCollectedIron(-5);
                AddCollectedRocks(-3);
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

    private void UpdateUI()
    {
        ironOreText.text = "Iron ore: " + ironOre;
        rocksText.text = "Lunar rocks: " + rocks;
    }
}
