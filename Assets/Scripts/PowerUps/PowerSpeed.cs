using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Duration")]
    [SerializeField] private float effectDuration = 20f;
    public void ApplyEffect(CharController player)
    {
        SpeedEffect Paylod = new(effectDuration);
        player.AddEffect(Paylod);
    }
}
