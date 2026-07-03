using UnityEngine;

public class PowerSlowMo : MonoBehaviour, IPowerUps
{

    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float slowDownTarget = 0.5f;
    

    public void ApplyEffect(PlayerEffects player)
    {
        SlowMoEffect Paylod = new(effectDuration,slowDownTarget);
        player.AddEffect(Paylod);
    }
}
