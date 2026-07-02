using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float snappingForce = 5f;
    [SerializeField] private float laneDistance = 3f;


    [Header("References")]
    [SerializeField] private InputActionReference moveLeftAction;
    [SerializeField] private InputActionReference moveRightAction;
    [SerializeField] private Fade _deathScreenFader;
    [SerializeField] private CanvasGroup _deathScreenCanvasGroup;

    [SerializeField] private float difficultyMultiplier=1.2f;

    private bool _isInvincible=false;
    private CharacterController _controller;
    private Lane _currentLane = Lane.middle;
    private float _lateralVelocity;
    private Queue<StatusEffect> _effectQueue = new();
    private StatusEffect _activeEffect = null;
    private float _currentSpeed;
    private int _score=0;
    private bool _isDead=false;

    private enum Lane
    {
        left = -1,
        middle = 0,
        right = 1
    }

    void Awake()
    {
        _currentSpeed=baseSpeed;
        _controller = GetComponent<CharacterController>();

    }

    void OnEnable()
    {
        moveLeftAction.action.performed += LeftAction;
        moveRightAction.action.performed += RightAction;

    }

    void OnDisable()
    {
        moveLeftAction.action.performed -= LeftAction;
        moveRightAction.action.performed -= RightAction;

    }

    private void RightAction(InputAction.CallbackContext context)
    {
        switch (_currentLane)
        {
            case Lane.left:
                _currentLane = Lane.middle;
                break;
            case Lane.middle:
                _currentLane = Lane.right;
                break;
        }
    }

    private void LeftAction(InputAction.CallbackContext context)
    {
        switch (_currentLane)
        {
            case Lane.right:
                _currentLane = Lane.middle;
                break;
            case Lane.middle:
                _currentLane = Lane.left;
                break;
        }
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }
        CalculateLaneVelocity();
        ApplyMovement();
        //UpDifficulty();
        HandleEffects();
    }

    private void CalculateLaneVelocity()
    {
        float targetXPosition = (int)_currentLane * laneDistance;
        float xDifference = targetXPosition - transform.position.x;
        _lateralVelocity = xDifference * snappingForce;
    }

    private void ApplyMovement()
    {
        
        var forwoard=_currentSpeed*Time.deltaTime;
        var dodge=_lateralVelocity*Time.unscaledDeltaTime;
        Vector3 moveVector = new(dodge, 0f, forwoard);
        _controller.Move(moveVector);
    }

    public void AddEffect(StatusEffect effect)
    {
        if (_activeEffect == null)
        {
            StartEffect(effect);
        }
        else
        {
            _effectQueue.Enqueue(effect);
        }
        
    }

    private void StartEffect(StatusEffect effect)
    {
        _activeEffect=effect;
        _activeEffect.OnApplyEffect(this);
    }

    private void StopEffect()
    {
        if (_activeEffect == null)
        {
            return;
        }
        _activeEffect.OnRemoveEffect(this);
        _activeEffect=null;
    }

    private void HandleEffects()
    {
        if (_activeEffect == null)
        {
            if (_effectQueue.Count == 0)
            {
                return;
            }
            StartEffect(_effectQueue.Dequeue());
        }
        _activeEffect.remainingTime -= Time.unscaledDeltaTime;
        if (_activeEffect.remainingTime <= 0)
        {
            StopEffect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (_isInvincible)
            {
                other.gameObject.SetActive(false);
                return;

            }
            PlayerGameOver();
        }
        else if (other.CompareTag("PowerUp"))
        {
            IPowerUps item = other.GetComponent<IPowerUps>();
            if (item != null)
            {
                item.ApplyEffect(this);
                other.gameObject.SetActive(false);
            }
        }
    }

    private void PlayerGameOver()
    {
        _isDead=true;
        Time.timeScale = 0f;
        if (_deathScreenCanvasGroup != null)
        {
            _deathScreenCanvasGroup.interactable = true;
            _deathScreenCanvasGroup.blocksRaycasts = true;
        }
        if (_deathScreenFader != null)
        {
            _deathScreenFader.TriggerFade(Fade.FadeType.In);
        }
    }

    public void ChangeInvincibility(bool state)
    {
        _isInvincible=state;
    }

    public void ChangeSpeed(float speed)
    {
        _currentSpeed+=speed;
    }

    private void UpDifficulty()
    {
        if (_score % 10 == 0 && _score!=0)
        {
            _currentSpeed*=difficultyMultiplier;
        }
    }

    public void UpScore()
    {
        _score++;
    }

    public int GetScore()
    {
        return _score;
    }

    public StatusEffect ActiveEffect=>_activeEffect;
    public StatusEffect NextEffect=>_effectQueue.Count>0?_effectQueue.Peek():null;



}