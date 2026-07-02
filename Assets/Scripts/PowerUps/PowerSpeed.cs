using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [SerializeField] private float effectDuration = 20f;
    public void ApplyEffect(CharController player)
    {
        SpeedEffect Paylod = new(effectDuration);
        player.AddEffect(Paylod);
    }
}
