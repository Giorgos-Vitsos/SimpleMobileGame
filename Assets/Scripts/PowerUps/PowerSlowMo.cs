using UnityEngine;

public class PowerSlowMo : MonoBehaviour, IPowerUps
{

    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float slowDownTarget = 0.5f;
    [SerializeField] private Sprite icon;


    public void ApplyEffect(PlayerEffects player)
    {
        SlowMoEffect Paylod = new(effectDuration, slowDownTarget,icon);
        player.AddEffect(Paylod);
    }
}
