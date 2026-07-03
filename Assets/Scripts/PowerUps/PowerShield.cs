using UnityEngine;

public class PowerShield : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 6f;
    public void ApplyEffect(PlayerEffects player)
    {
        ShieldEffect Paylod = new(effectDuration);
        player.AddEffect(Paylod);
    }

}
