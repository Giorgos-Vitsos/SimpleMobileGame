using UnityEngine;
using System.IO; 
using System;

public static class SaveManager
{
    private static string SaveFilePath => Application.persistentDataPath + "/gamesave.json";

    public static void SaveGameState(GameStateData dataToSave)
    {
        try
        {
            string jsonText = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(SaveFilePath, jsonText);
            Debug.Log("Game successfully saved at: " + SaveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public static GameStateData LoadGameState()
    {
        if (File.Exists(SaveFilePath))
        {
            try 
            {
                string jsonText = File.ReadAllText(SaveFilePath);
                GameStateData loadedData = JsonUtility.FromJson<GameStateData>(jsonText);
                Debug.Log("Game loaded successfully.");
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load save file, it might be corrupted: " + e.Message);
                return null;
            }
        }
        
        Debug.LogWarning("No save file found at " + SaveFilePath);
        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("Save file safely deleted.");
        }
    }
}