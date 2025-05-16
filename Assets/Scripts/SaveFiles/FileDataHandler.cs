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
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"Save file not found: {fullPath}. Returning default GameData.");
            return new GameData();
        }

        try
        {
            string jsonData = File.ReadAllText(fullPath);
            Debug.Log($"Loaded JSON: {jsonData}");
            return JsonUtility.FromJson<GameData>(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load file: {e.Message}");
            return new GameData();
        }
    }

    // Save data
    public void Save(GameData data)
    {
        if (data == null)
        {
            Debug.LogError("Save failed: GameData is null!");
            return;
        }

        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(fullPath, jsonData);
            Debug.Log($"Game Saved! {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving file: {e.Message}");
        }
    }

}
