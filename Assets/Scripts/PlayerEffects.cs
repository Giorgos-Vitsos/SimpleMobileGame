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

    private void Awake()
    {
        CurrentSpeed = baseSpeed;
    }

    private void Update()
    {
        HandleEffects();
    }

    public void SetInvincibility(bool state) => IsInvincible = state;
    
    public void ModifySpeed(float amount) => CurrentSpeed += amount;
    
    public void MultiplySpeed(float multiplier) => CurrentSpeed *= multiplier;

    public void AddEffect(StatusEffect effect)
    {
        if (_activeEffect == null) StartEffect(effect);
        else _effectQueue.Enqueue(effect);
    }

    private void StartEffect(StatusEffect effect)
    {
        _activeEffect = effect;
        _activeEffect.OnApplyEffect(this); 
    }

    private void StopEffect()
    {
        if (_activeEffect == null) return;
        _activeEffect.OnRemoveEffect(this);
        _activeEffect = null;
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
}