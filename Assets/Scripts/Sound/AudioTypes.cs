using UnityEngine;

public enum SoundType
{
    Running,
    PickupPowerup,
    Dodge,
    ButtonClick,
    PlayerDeath,
    ButtonHover,
    ObstacleBreak,
    PlayersBodyHit
}

[System.Serializable]
public struct SoundGroup
{
    public string groupName; 
    public SoundType type;
    public AudioClip[] clips; 
    
    [Range(0f, 1f)]
    public float volume; 
    public bool scalesWithTime;
}