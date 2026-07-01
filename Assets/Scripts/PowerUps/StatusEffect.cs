using UnityEngine;

public abstract class StatusEffect
{
    public float duration;
    public float remainingTime;

    public abstract void OnApplyEffect(CharController player);
    public abstract void OnRemoveEffect(CharController player);
}

public class ShieldEffect : StatusEffect
{
    public ShieldEffect(float duration)
    {
        this.duration=duration;
        remainingTime=duration;
    }

    public override void OnApplyEffect(CharController player)
    {
        Debug.Log($"ShieldIsOn for {duration}");
    }

    public override void OnRemoveEffect(CharController player)
    {
        Debug.Log($"ShieldIsOff");
    }
}