using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float speedChange = 3f;
    [SerializeField] private bool grantShield = true;
    public void ApplyEffect(PlayerEffects player)
    {
        SpeedEffect Paylod = new(effectDuration,speedChange);
        player.AddEffect(Paylod);
        if (grantShield)
        {
            ShieldEffect Paylod2 = new(effectDuration);
            player.AddEffect(Paylod2);
        }
    }
}
