using UnityEngine;

public class PowerSlowMo : MonoBehaviour, IPowerUps
{

    [SerializeField] private float effectDuration = 10f;
    public void ApplyEffect(CharController player)
    {
        SlowMoEffect Paylod = new(effectDuration);
        player.AddEffect(Paylod);
    }
}
