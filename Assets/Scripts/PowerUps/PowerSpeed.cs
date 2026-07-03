using UnityEngine;

public class PowerSpeed : MonoBehaviour, IPowerUps
{
    [Header("PowerUp Settings")]
    [SerializeField] private float effectDuration = 5f;
    [SerializeField] private float speedChange = 3f;
    [SerializeField] private bool grantShield = true;
     [SerializeField] private Sprite icon;
    public void ApplyEffect(PlayerEffects player)
    {
        SpeedEffect Paylod1 = new(effectDuration,speedChange,icon);
        ShieldEffect Paylod2 = new(effectDuration,null);
        if (grantShield)
        {
            CombinedEffect Paylod=new(effectDuration,icon,Paylod1,Paylod2);
            player.AddEffect(Paylod);
            return;
        }
        player.AddEffect(Paylod1);

    }
}
