using System.Collections.Generic;

[System.Serializable] 
public class GameStateData
{
    public int currentScore;
    public PlayerMovement.Lane currentLane; 
    public float playerZPosition;
    public float nextSpawnPos; 
    public int currentMaxObstaclesPerTrack;

    public float playerCurrentSpeed;
    public SavedEffectData activeEffect;
    public List<SavedEffectData> effectQueue = new List<SavedEffectData>();

    public List<int> trackZPositionsRounded = new List<int>();

    public List<SavedTrackItems> trackItems = new List<SavedTrackItems>();
}

[System.Serializable]
public class SavedTrackItems
{
    
    public int trackZPositionRounded; 
    public List<string> itemPrefabNames = new List<string>();
    public List<int> spawnPointIndices = new List<int>();
}

public enum EffectType { Shield, SlowMo, Speed, Combined }

[System.Serializable]
public class SavedEffectData
{
    public EffectType type;
    public float remainingTime;
    
    // Αποθηκεύει το _speed Ή το _slowDownTarget ανάλογα το effect
    public float floatParameter; 
    
    // Αν είναι CombinedEffect, αποθηκεύει τα υπο-effects του
    public List<SavedEffectData> nestedEffects = new List<SavedEffectData>(); 
}