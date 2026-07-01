using UnityEngine;

public abstract class StatusEffect
{
    public float duration;
    public float remainingTime;

    protected StatusEffect(float time)
    {
        duration=time;
        remainingTime=time;
    }
    public abstract void OnApplyEffect(CharController player);
    public abstract void OnRemoveEffect(CharController player);
}

public class ShieldEffect : StatusEffect
{
    public ShieldEffect(float time):base(time){}
    
    public override void OnApplyEffect(CharController player)
    {
        Debug.Log($"Shield is on for {duration}");
    }

    public override void OnRemoveEffect(CharController player)
    {
        Debug.Log($"ShieldIsOff");
    }
}

public class SlowMoEffect : StatusEffect
{
    public SlowMoEffect(float time):base(time){}

    public override void OnApplyEffect(CharController player)
    {
        Debug.Log($"SlowMo is on for {duration}");
    }

    public override void OnRemoveEffect(CharController player)
    {
        Debug.Log($"SlowMo is off");
    }
}

public class SpeedEffect : StatusEffect
{
    public SpeedEffect(float time):base(time){}

    public override void OnApplyEffect(CharController player)
    {
        Debug.Log($"Speed is on for {duration}");
    }

    public override void OnRemoveEffect(CharController player)
    {
        Debug.Log($"Speed is off");
    }
}