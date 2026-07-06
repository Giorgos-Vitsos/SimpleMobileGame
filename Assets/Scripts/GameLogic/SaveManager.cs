using UnityEngine;
using System.IO; 

public static class SaveManager
{
    private static string SaveFilePath => Application.persistentDataPath + "/gamesave.json";

    public static void SaveGameState(GameStateData dataToSave)
    {
       
        string jsonText = JsonUtility.ToJson(dataToSave, true);

        File.WriteAllText(SaveFilePath, jsonText);
        
        Debug.Log("Game successfully saved at: " + SaveFilePath);
    }

    public static GameStateData LoadGameState()
    {

        if (File.Exists(SaveFilePath))
        {

            string jsonText = File.ReadAllText(SaveFilePath);

            GameStateData loadedData = JsonUtility.FromJson<GameStateData>(jsonText);
            return loadedData;
        }

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