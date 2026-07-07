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
    private bool _gamePaused=false;

    private void Awake()
    {
        CurrentSpeed = baseSpeed;
    }

    private void Update()
    {
        if (_gamePaused) return;
        HandleEffects();
    }

    private void OnEnable()
    {
        GameEvents.OnPauseStateChanged+=StopEffects;
        GameEvents.OnGatherSaveData += InjectData;
        GameEvents.OnRestoreSaveData += RestoreData;

    }

    private void OnDisable()
    {
        GameEvents.OnPauseStateChanged-=StopEffects;
        GameEvents.OnGatherSaveData -= InjectData;
        GameEvents.OnRestoreSaveData -= RestoreData;

    }

    public void SetInvincibility(bool state) => IsInvincible = state;
    
    public void ModifySpeed(float amount) => CurrentSpeed += amount;
    
    public void MultiplySpeed(float multiplier) => CurrentSpeed *= multiplier;

    public void AddEffect(StatusEffect effect)
    {
        if (_activeEffect == null) StartEffect(effect);
        else _effectQueue.Enqueue(effect);
        UpdateHUD();
    }

    private void StartEffect(StatusEffect effect)
    {
        _activeEffect = effect;
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
            StartEffect(_effectQueue.Dequeue());
        }
        
        _activeEffect.remainingTime -= Time.unscaledDeltaTime;
        if (_activeEffect.remainingTime <= 0) StopEffect();
    }

    private void UpdateHUD()
    {
        StatusEffect nextInQueue = _effectQueue.Count > 0 ? _effectQueue.Peek() : null;
        GameEvents.OnEffectsHUDUpdated?.Invoke(_activeEffect, nextInQueue);
    }

    private void StopEffects(bool state)=>_gamePaused=state;

    private void InjectData(GameStateData snapshot)
    {

        if (_activeEffect != null)
        {
            _activeEffect.OnRemoveEffect(this);
        }

        // 2. Save the true, unmodified speed
        snapshot.playerCurrentSpeed = CurrentSpeed;

        // 3. Instantly re-apply the effect so gameplay continues uninterrupted
        if (_activeEffect != null)
        {
            _activeEffect.OnApplyEffect(this);
        }
        
        snapshot.activeEffect = _activeEffect?.GetSaveData();

        snapshot.effectQueue.Clear();
        foreach (var effect in _effectQueue)
        {
            snapshot.effectQueue.Add(effect.GetSaveData());
        }
    }

    private void RestoreData(GameStateData data)
    {
        // 1. Καθαρίζουμε τα πάντα
        if (_activeEffect != null) StopEffect();
        _effectQueue.Clear();

        // 2. Επαναφορά base ταχύτητας
        CurrentSpeed = data.playerCurrentSpeed;

        // 3. Επαναφορά Active Effect
        if (data.activeEffect != null)
        {
            StatusEffect loadedEffect = RebuildEffect(data.activeEffect);
            if (loadedEffect != null) StartEffect(loadedEffect);
        }

        // 4. Επαναφορά Ουράς
        foreach (var savedData in data.effectQueue)
        {
            StatusEffect loadedEffect = RebuildEffect(savedData);
            if (loadedEffect != null) _effectQueue.Enqueue(loadedEffect);
        }

        UpdateHUD();
    }

    // Factory method που ξαναφτιάχνει το σωστό αντικείμενο από το JSON
    private StatusEffect RebuildEffect(SavedEffectData data)
    {
        // Σημείωση: Περνάμε null για το Sprite icon κατά το Load, διότι τα Sprites 
        // δεν αποθηκεύονται. Το HUD σου πρέπει να ελέγχει αν icon != null.
        switch (data.type)
        {
            case EffectType.Shield:
                var shield = new ShieldEffect(data.remainingTime, null);
                shield.remainingTime = data.remainingTime;
                return shield;
                
            case EffectType.SlowMo:
                var slow = new SlowMoEffect(data.remainingTime, data.floatParameter, null);
                slow.remainingTime = data.remainingTime;
                return slow;
                
            case EffectType.Speed:
                var speed = new SpeedEffect(data.remainingTime, data.floatParameter, null);
                speed.remainingTime = data.remainingTime;
                return speed;
                
            case EffectType.Combined:
                List<StatusEffect> nested = new List<StatusEffect>();
                foreach (var nestedData in data.nestedEffects)
                {
                    nested.Add(RebuildEffect(nestedData));
                }
                var combined = new CombinedEffect(data.remainingTime, null, nested.ToArray());
                combined.remainingTime = data.remainingTime;
                return combined;
                
            default:
                return null;
        }
    }
}