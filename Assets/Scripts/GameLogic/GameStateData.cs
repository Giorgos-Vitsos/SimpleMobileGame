using System.Collections.Generic;

[System.Serializable] 
public class GameStateData
{
    // --- PLAYER STATS ---
    public int currentScore;
    public PlayerMovement.Lane currentLane; 
    public float playerZPosition;
    public float nextSpawnPos; 
    public int currentMaxObstaclesPerTrack;

    // 1. Το TrackPoolManager θα γεμίζει ΑΥΤΗ τη λίστα (Μόνο τις θέσεις των δρόμων)
    public List<float> trackZPositions = new List<float>();

    // 2. Το SpawnedPoolManager θα γεμίζει ΑΥΤΗ τη λίστα (Μόνο τα items)
    public List<SavedTrackItems> trackItems = new List<SavedTrackItems>();
}

// Το mini-blueprint αποκλειστικά για τα items ενός δρόμου
[System.Serializable]
public class SavedTrackItems
{
    // Αυτό είναι το "κλειδί" για να ξέρουμε σε ποιον δρόμο ανήκουν τα items όταν κάνουμε Load
    public float trackZPosition; 
    
    public List<int> itemPrefabIDs = new List<int>();
    public List<int> spawnPointIndices = new List<int>();
}