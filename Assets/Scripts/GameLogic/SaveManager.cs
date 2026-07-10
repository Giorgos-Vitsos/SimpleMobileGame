using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;

public static class SaveManager
{
    private static string SaveFilePath => Application.persistentDataPath + "/gamesave.json";//save file location

    private const string SecretKey = "ThisIsAStrongPassword";

    public static void SaveGameState(GameStateData dataToSave)
    {
        try
        {
            string jsonText = JsonUtility.ToJson(dataToSave, true);//converts data to json

            string checksum = GenerateChecksum(jsonText);//we generate secret hash

            //we add the hash add the end
            string combinedData = jsonText + '|' + checksum;

            //encrypt everything
            string encryptedData = EncryptDecryptXOR(combinedData);

            File.WriteAllText(SaveFilePath, encryptedData);//we try and write save file
            Debug.Log("Game successfully secured and saved at: " + SaveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public static GameStateData LoadGameState()
    {
        if (SaveExists())
        {
            try
            {

                string encryptedText = File.ReadAllText(SaveFilePath);//read save file
                string decryptedText = EncryptDecryptXOR(encryptedText);//decrypt
                string[] parts = decryptedText.Split('|');//extract hash
                if (parts.Length != 2)
                {
                    Debug.LogError("Save file format is invalid! It may have been corrupted.");
                    return null;
                }

                string jsonText = parts[0];
                string loadedChecksum = parts[1];

                //check if hash matches
                string calculatedChecksum = GenerateChecksum(jsonText);
                if (calculatedChecksum != loadedChecksum)
                {
                    Debug.LogError("Save file has been tampered with.");
                    return null;
                }

                //if not tampered load game
                GameStateData loadedData = JsonUtility.FromJson<GameStateData>(jsonText);
                Debug.Log("Game loaded and verified successfully.");
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
        if (SaveExists())
        {
            File.Delete(SaveFilePath);
            Debug.Log("Save file safely deleted.");
        }
    }

    public static bool SaveExists() => File.Exists(SaveFilePath);

    private static string EncryptDecryptXOR(string data)
    {
        StringBuilder output = new();
        for (int i = 0; i < data.Length; i++)
        {
            output.Append((char)(data[i] ^ SecretKey[i % SecretKey.Length]));//XOR
        }
        return output.ToString();
    }

    private static string GenerateChecksum(string originalData)
    {
        string combined = originalData + SecretKey;
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(bytes);
        }
    }
}