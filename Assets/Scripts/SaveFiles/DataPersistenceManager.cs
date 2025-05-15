using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    public static DataPersistenceManager instance { get; private set; }
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one data persistence manager found in scene.");
        }
        instance = this;
    }

    private void Start()
    {
        // Initialize data handlers for each GameManager dynamically
        dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            GameManager gameManager = obj as GameManager;
            if (gameManager != null)
            {
                // Each GameManager gets a separate FileDataHandler instance
                FileDataHandler localDataHandler = new FileDataHandler(Application.persistentDataPath, gameManager.FileName, false);
                GameData loadedData = localDataHandler.Load();

                if (loadedData != null)
                {
                    obj.LoadData(loadedData);
                }
                else
                {
                    Debug.Log($"No save found for {gameManager.FileName}. Creating new game data.");
                    obj.LoadData(new GameData()); // Initialize new data for this manager
                }
            }
        }
    }


    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        // New game if no data found
        if (this.gameData == null)
        {
            Debug.Log("No data found. Initializing data.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log($"Loaded: Iron = {gameData.ironOre}, Rock = {gameData.rocks}, Water = {gameData.water}/30");
    }

    public void SaveGame()
    {
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            obj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
        Debug.Log($"Saved: Iron = {gameData.ironOre}, Rock = {gameData.rocks}, Water = {gameData.water}/30");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects); 
    }

    public GameData GetGameData()
    {
        return gameData;
    }

}
