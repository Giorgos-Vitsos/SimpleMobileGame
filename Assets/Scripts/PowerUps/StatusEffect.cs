using UnityEngine;

public abstract class StatusEffect
{
    public float duration;
    public float remainingTime;

    public Sprite Icon { get; private set; }

    protected StatusEffect(float time, Sprite icon)
    {
        duration = time;
        remainingTime = time;
        Icon = icon;
    }
    public abstract void OnApplyEffect(PlayerEffects player);
    public abstract void OnRemoveEffect(PlayerEffects player);

    public abstract SavedEffectData GetSaveData();
}

public class ShieldEffect : StatusEffect
{
    public ShieldEffect(float time, Sprite icon) : base(time, icon) { }

    public override void OnApplyEffect(PlayerEffects player)
    {
        player.SetInvincibility(true);
    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        player.SetInvincibility(false);
    }

    public override SavedEffectData GetSaveData()
    {
        return new SavedEffectData { type = EffectType.Shield, remainingTime = remainingTime };
    }
}

public class SlowMoEffect : StatusEffect
{
    private float _slowDownTarget;
    public SlowMoEffect(float time, float slowDownTarget, Sprite icon) : base(time, icon)
    {
        _slowDownTarget = slowDownTarget;
    }

    public override void OnApplyEffect(PlayerEffects player)
    {
        Time.timeScale = _slowDownTarget;

    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        Time.timeScale = 1f;
    }

    public override SavedEffectData GetSaveData()
    {
        return new SavedEffectData { type = EffectType.SlowMo, remainingTime = remainingTime, floatParameter = _slowDownTarget };
    }
}

public class SpeedEffect : StatusEffect
{
    private float _speed;
    private ShieldEffect _shield;
    public SpeedEffect(float time, float speed, Sprite icon) : base(time, icon)
    {
        _speed = speed;

    }

    public override void OnApplyEffect(PlayerEffects player)
    {
        player.ModifySpeed(_speed);

    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        player.ModifySpeed(-_speed);
    }

    public override SavedEffectData GetSaveData()
    {
        return new SavedEffectData { type = EffectType.Speed, remainingTime = remainingTime, floatParameter = _speed };
    }
}

public class CombinedEffect : StatusEffect
{
    private StatusEffect[] _effects;

    public CombinedEffect(float time, Sprite icon, params StatusEffect[] effects) : base(time, icon)
    {
        _effects = effects;
    }

    public override void OnApplyEffect(PlayerEffects player)
    {
        foreach (var effect in _effects)
        {
            effect.OnApplyEffect(player);
        }
    }

    public override void OnRemoveEffect(PlayerEffects player)
    {
        foreach (var effect in _effects)
        {
            effect.OnRemoveEffect(player);
        }
    }

    public override SavedEffectData GetSaveData()
    {
        var data = new SavedEffectData { type = EffectType.Combined, remainingTime = remainingTime };
        foreach (var effect in _effects)//save each effect
        {

            var rawData = effect.GetSaveData();
            data.nestedEffects.Add(new SavedSubEffectData
            {
                type = rawData.type,
                remainingTime = rawData.remainingTime,
                floatParameter = rawData.floatParameter
            });
        }
        return data;
    }
}