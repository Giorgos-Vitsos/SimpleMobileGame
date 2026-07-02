using UnityEngine;

public abstract class StatusEffect
{
    public float duration;
    public float remainingTime;

    protected StatusEffect(float time)
    {
        duration = time;
        remainingTime = time;
    }
    public abstract void OnApplyEffect(CharController player);
    public abstract void OnRemoveEffect(CharController player);
}

public class ShieldEffect : StatusEffect
{
    public ShieldEffect(float time) : base(time) { }

    public override void OnApplyEffect(CharController player)
    {
        player.ChangeInvincibility(true);
    }

    public override void OnRemoveEffect(CharController player)
    {
        player.ChangeInvincibility(false);
    }
}

public class SlowMoEffect : StatusEffect
{
    private float _slowDownTarget;
    public SlowMoEffect(float time,float slowDownTarget) : base(time)
    {
        _slowDownTarget=slowDownTarget;
    }

    public override void OnApplyEffect(CharController player)
    {
        Time.timeScale=_slowDownTarget;

    }

    public override void OnRemoveEffect(CharController player)
    {
        Time.timeScale=1f;
    }
}

public class SpeedEffect : StatusEffect
{
    private float _speed;
    public SpeedEffect(float time,float speed) : base(time)
    {
        _speed=speed;
    }

    public override void OnApplyEffect(CharController player)
    {
        player.ChangeSpeed(_speed);
    }

    public override void OnRemoveEffect(CharController player)
    {
        player.ChangeSpeed(-_speed);
    }
}