using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    // Filepath: C:\Users\{YourUserName}\AppData\LocalLow\DefaultCompany\SD4981_Space
    private string dataDirPath;
    public string dataFileName;

    // Marks file path, file name and encryption
    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    // Load data
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string jsonData = File.ReadAllText(fullPath);
                loadedData = JsonUtility.FromJson<GameData>(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
            }
        }

        if (loadedData == null)
        {
            Debug.LogError("Loaded data is null, initializing new GameData.");
            loadedData = new GameData(); // Create default data if load fails
        }

        return loadedData;
    }

    // Save data
    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName); // Uses assigned filename format

        try
        {
            string serializedData = JsonUtility.ToJson(data, true); // Default JSON serialization

            // Determine file format from filename
            string fileExtension = Path.GetExtension(dataFileName).ToLower();

            if (fileExtension == ".game")
            {
                // Example: Apply a basic encryption or modification for .game format (optional)
                serializedData = "GAME_SAVE_FORMAT\n" + serializedData;
            }

            File.WriteAllText(fullPath, serializedData);
            Debug.Log($"Game Saved! File Path: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data: " + e.Message);
        }
    }
}
