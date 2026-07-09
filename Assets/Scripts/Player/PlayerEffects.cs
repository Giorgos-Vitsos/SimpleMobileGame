using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseSpeed = 5f;

    public float CurrentSpeed { get; private set; }
    public bool IsInvincible { get; private set; }

    private Queue<StatusEffect> _effectQueue = new();
    private StatusEffect _activeEffect = null;
    private bool _gamePaused = false;
    private float _lastEffectTime = -1f;
    private const float WARNING_TIME = 2f;//warning before effect disapears
    private const float MAX_SPEED = 50f;
    private bool _isWarningSent = false;//tracks expiration warning 
    private bool _isDead = false;

    private void Awake()
    {
        CurrentSpeed = baseSpeed;
    }

    private void Update()
    {
        if (_gamePaused || _isDead) return;
        GameEvents.OnSpeedChanged?.Invoke(CurrentSpeed, MAX_SPEED);
        HandleEffects();
    }

    private void OnEnable()
    {
        GameEvents.OnPauseStateChanged += StopEffects;
        GameEvents.OnGatherSaveData += InjectData;
        GameEvents.OnRestoreSaveData += RestoreData;
        GameEvents.OnPlayerDeath += HandleDeath;
        GameEvents.OnRestartRequest += HandleRespawn;
    }

    private void OnDisable()
    {
        GameEvents.OnPauseStateChanged -= StopEffects;
        GameEvents.OnGatherSaveData -= InjectData;
        GameEvents.OnRestoreSaveData -= RestoreData;
        GameEvents.OnPlayerDeath -= HandleDeath;
        GameEvents.OnRestartRequest -= HandleRespawn;
    }

    public void SetInvincibility(bool state) => IsInvincible = state;

    public void ModifySpeed(float amount) => CurrentSpeed += amount;

    public void MultiplySpeed(float multiplier) => CurrentSpeed *= multiplier;

    public void AddEffect(StatusEffect effect)
    {
        if (Time.unscaledTime - _lastEffectTime < 0.1f) return;//fixes bug where collider was counting 2 effects on load
        _lastEffectTime = Time.unscaledTime;
        if (_activeEffect == null) StartEffect(effect);
        else _effectQueue.Enqueue(effect);
        UpdateHUD();
    }

    private void StartEffect(StatusEffect effect)
    {
        _activeEffect = effect;
        _isWarningSent = false;
        _activeEffect.OnApplyEffect(this);
        UpdateHUD();
    }

    private void StopEffect()
    {
        if (_activeEffect == null) return;
        _activeEffect.OnRemoveEffect(this);
        _activeEffect = null;
        UpdateHUD();
    }

    private void HandleEffects()
    {
        if (_activeEffect == null)
        {
            if (_effectQueue.Count == 0) return;
            StartEffect(_effectQueue.Dequeue());//we start next effect
        }

        _activeEffect.remainingTime -= Time.unscaledDeltaTime;//reduce remaining time
        if (!_isWarningSent && _activeEffect.remainingTime <= WARNING_TIME)//send warning
        {
            _isWarningSent = true;
            UpdateHUD();
        }
        if (_activeEffect.remainingTime <= 0) StopEffect();
    }

    private void UpdateHUD()
    {
        Sprite activeSprite = _activeEffect?.Icon;
        Sprite queueSprite = _effectQueue.Count > 0 ? _effectQueue.Peek().Icon : null;

        bool isWarning = _isWarningSent;

        GameEvents.OnEffectsHUDUpdated?.Invoke(activeSprite, queueSprite, isWarning);
    }

    private void StopEffects(bool state) => _gamePaused = state;

    private void InjectData(GameStateData snapshot)
    {

        bool _isSlowMo = _activeEffect is SlowMoEffect;

        if (_activeEffect != null && !_isSlowMo)//we remove effects that change speed
        {
            _activeEffect.OnRemoveEffect(this);
        }

        snapshot.playerCurrentSpeed = CurrentSpeed;//we extract speed

        if (_activeEffect != null && !_isSlowMo)//we reapply the effect
        {
            _activeEffect.OnApplyEffect(this);
        }

        //we save all current effects
        snapshot.activeEffect = _activeEffect?.GetSaveData();
        snapshot.effectQueue.Clear();//garbage cleaning
        foreach (var effect in _effectQueue)
        {
            snapshot.effectQueue.Add(effect.GetSaveData());
        }
    }

    private void RestoreData(GameStateData data)
    {

        //we stop everything
        if (_activeEffect != null) StopEffect();
        _effectQueue.Clear();

        CurrentSpeed = data.playerCurrentSpeed;

        //we load all effects
        if (data.activeEffect != null)
        {
            StatusEffect loadedEffect = RebuildEffect(data.activeEffect);
            if (loadedEffect != null) StartEffect(loadedEffect);
        }

        foreach (var savedData in data.effectQueue)
        {
            StatusEffect loadedEffect = RebuildEffect(savedData);
            if (loadedEffect != null) _effectQueue.Enqueue(loadedEffect);
        }

        UpdateHUD();
    }

    //converts data into effects
    private StatusEffect RebuildEffect(SavedEffectData data)
    {
        Sprite loadedIcon = Resources.Load<Sprite>(data.type.ToString());//we get the icon

        switch (data.type)
        {
            case EffectType.Shield:
                var shield = new ShieldEffect(data.remainingTime, loadedIcon);
                shield.remainingTime = data.remainingTime;
                return shield;

            case EffectType.SlowMo:
                var slow = new SlowMoEffect(data.remainingTime, data.floatParameter, loadedIcon);
                slow.remainingTime = data.remainingTime;
                return slow;

            case EffectType.Speed:
                var speed = new SpeedEffect(data.remainingTime, data.floatParameter, loadedIcon);
                speed.remainingTime = data.remainingTime;
                return speed;

            case EffectType.Combined:
                List<StatusEffect> nested = new List<StatusEffect>();
                foreach (var nestedData in data.nestedEffects)
                {
                    var tempFormat = new SavedEffectData
                    {
                        type = nestedData.type,
                        remainingTime = nestedData.remainingTime,
                        floatParameter = nestedData.floatParameter
                    };
                    nested.Add(RebuildEffect(tempFormat));
                }
                var combined = new CombinedEffect(data.remainingTime, loadedIcon, nested.ToArray());
                combined.remainingTime = data.remainingTime;
                return combined;

            default:
                return null;
        }
    }

    private void HandleDeath() => _isDead = true;
    private void HandleRespawn()
    {
        _isDead = false;
        _gamePaused = false;
    }
}