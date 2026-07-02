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

    private bool _isInvincible=false;
    private CharacterController _controller;
    private Lane _currentLane = Lane.middle;
    private float _lateralVelocity;
    private Queue<StatusEffect> _effectQueue = new();
    private StatusEffect _activeEffect = null;
    private float _currentSpeed;

    private enum Lane
    {
        left = -1,
        middle = 0,
        right = 1
    }

    void Awake()
    {
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

        CalculateLaneVelocity();
        ApplyMovement();
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
        Vector3 moveVector = new(_lateralVelocity, 0f, baseSpeed);
        _controller.Move(moveVector * Time.deltaTime);
    }

    public void AddEffect(StatusEffect effect)
    {
        _effectQueue.Enqueue(effect);
    }

    private void HandleEffects()
    {
        if (_activeEffect == null)
        {
            if (_effectQueue.Count == 0)
            {
                return;
            }
            _activeEffect = _effectQueue.Dequeue();
            _activeEffect.OnApplyEffect(this);
        }
        _activeEffect.remainingTime -= Time.deltaTime;
        if (_activeEffect.remainingTime <= 0)
        {
            _activeEffect.OnRemoveEffect(this);
            _activeEffect = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("CRASH! Game Over.");
            //Time.timeScale = 0f;
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


}
