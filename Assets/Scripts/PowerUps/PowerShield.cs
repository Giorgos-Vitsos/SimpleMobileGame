using System;
using UnityEngine;

public class PowerShield : MonoBehaviour,IPowerUps
{

    [SerializeField] private float shieldDuration=5f;
    public void ApplyEffect(CharController player)
    {
        ShieldEffect Paylod=new ShieldEffect(shieldDuration);
        player.AddEffect(Paylod);
    }

}
