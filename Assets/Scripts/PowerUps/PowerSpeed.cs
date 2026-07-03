using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float speedChange = 3f;
    [SerializeField] private bool grantShield = true;
    public void ApplyEffect(PlayerEffects player)
    {
        SpeedEffect Paylod1 = new(effectDuration,speedChange);
        ShieldEffect Paylod2 = new(effectDuration);
        if (grantShield)
        {
            CombinedEffect Paylod=new(effectDuration,Paylod1,Paylod2);
            player.AddEffect(Paylod);
            return;
        }
        player.AddEffect(Paylod1);

    }
}
