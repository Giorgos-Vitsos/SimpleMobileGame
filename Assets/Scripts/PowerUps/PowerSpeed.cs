using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float speedChange = 3f;
    public void ApplyEffect(CharController player)
    {
        SpeedEffect Paylod = new(effectDuration,speedChange);
        player.AddEffect(Paylod);
    }
}
