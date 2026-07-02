using UnityEngine;

public class PowerShield : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    public void ApplyEffect(CharController player)
    {
        ShieldEffect Paylod = new(effectDuration);
        player.AddEffect(Paylod);
    }

}
