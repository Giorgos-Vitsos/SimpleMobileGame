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
    public List<SavedEffectData> effectQueue = new();

    public List<int> trackZPositionsRounded = new();

    public List<SavedTrackItems> trackItems = new();
}

[System.Serializable]
public class SavedTrackItems
{
    
    public int trackZPositionRounded; 
    public List<string> itemPrefabNames = new ();
    public List<int> spawnPointIndices = new ();
}

public enum EffectType { Shield, SlowMo, Speed, Combined }

[System.Serializable]
public class SavedEffectData
{
    public EffectType type;
    public float remainingTime;
    public float floatParameter;
    
    public List<SavedSubEffectData> nestedEffects = new List<SavedSubEffectData>(); 
}

[System.Serializable]
public class SavedSubEffectData
{
    public EffectType type;
    public float remainingTime;
    public float floatParameter;
}