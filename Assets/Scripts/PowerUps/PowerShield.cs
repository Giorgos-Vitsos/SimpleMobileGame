using UnityEngine;

public class PowerShield : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 6f;
    [SerializeField] private Sprite icon;
    public void ApplyEffect(PlayerEffects player)
    {
        ShieldEffect Paylod = new(effectDuration, icon);
        player.AddEffect(Paylod);
    }

}
