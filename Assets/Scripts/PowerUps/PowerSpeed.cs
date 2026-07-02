using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 20f;
    [SerializeField] private float speedChange = 5f;
    public void ApplyEffect(CharController player)
    {
        SpeedEffect Paylod = new(effectDuration,speedChange);
        player.AddEffect(Paylod);
    }
}
