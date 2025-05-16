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

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, "GlobalSave.game"); // Ensure a file handler exists
        dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            GameManager gameManager = obj as GameManager;
            if (gameManager != null)
            {
                gameManager.LoadGame(); // Each GameManager loads its own file
            }
        }
    }


    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        if (dataHandler == null)
        {
            Debug.LogError("DataHandler is null. Ensure it's initialized properly.");
            return;
        }

        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.LogWarning("No data found. Creating new GameData.");
            this.gameData = new GameData();
            dataHandler.Save(gameData); // Ensure new data is saved immediately
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log($"Loaded: Iron = {gameData.ironOre}, Rock = {gameData.rocks}, Water = {gameData.water}/30");
    }


    public void SaveGame()
    {
        // Ensure gameData is initialized before attempting to save
        if (gameData == null)
        {
            Debug.LogWarning("SaveGame failed: gameData is null! Initializing new GameData.");
            gameData = new GameData(); // Prevents null errors
        }

        // Ensure dataPersistenceObjects exist before looping
        if (dataPersistenceObjects == null || dataPersistenceObjects.Count == 0)
        {
            Debug.LogError("No data persistence objects found! Make sure GameManagers are initialized.");
            return; // Prevents further execution
        }

        // Save data for each GameManager separately
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            GameManager gameManager = obj as GameManager;
            if (gameManager != null)
            {
                gameManager.SaveGame(); // Calls each GameManager's individual save function
            }
        }

        Debug.Log("All GameManagers have saved their data.");
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
