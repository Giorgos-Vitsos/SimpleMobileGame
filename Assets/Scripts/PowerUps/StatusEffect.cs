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
    public abstract void OnApplyEffect(PlayerEffects player);
    public abstract void OnRemoveEffect(PlayerEffects player);
}

public class ShieldEffect : StatusEffect
{
    public ShieldEffect(float time) : base(time) { }

    public override void OnApplyEffect(PlayerEffects player)
    {
        player.SetInvincibility(true);
    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        player.SetInvincibility(false);
    }
}

public class SlowMoEffect : StatusEffect
{
    private float _slowDownTarget;
    public SlowMoEffect(float time,float slowDownTarget) : base(time)
    {
        _slowDownTarget=slowDownTarget;
    }

    public override void OnApplyEffect(PlayerEffects player)
    {
        Time.timeScale=_slowDownTarget;

    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        Time.timeScale=1f;
    }
}

public class SpeedEffect : StatusEffect
{
    private float _speed;
    private ShieldEffect _shield;
    public SpeedEffect(float time,float speed) : base(time)
    {
        _speed=speed;
        _shield=new(time);

    }

    public override void OnApplyEffect(PlayerEffects player)
    {
        player.ModifySpeed(_speed);
        
    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        player.ModifySpeed(-_speed);
    }
}