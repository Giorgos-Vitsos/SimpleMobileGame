using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float snappingForce = 5f;
    [SerializeField] private float laneDistance = 3f;

    [Header("References")]
    [SerializeField] private InputActionReference moveLeftAction;
    [SerializeField] private InputActionReference moveRightAction;

    private CharacterController _controller;
    private PlayerEffects _effects;
    private Lane _currentLane = Lane.middle;
    private float _lateralVelocity;
    private bool _canMove = true;

    private enum Lane { left = -1, middle = 0, right = 1 }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _effects = GetComponent<PlayerEffects>();
    }

    private void OnEnable()
    {
        moveLeftAction.action.performed += LeftAction;
        moveRightAction.action.performed += RightAction;
        GameEvents.OnPlayerDeath += DisableMovement;
        GameEvents.OnPauseStateChanged +=SwitchStateMovement;
    }

    private void OnDisable()
    {
        moveLeftAction.action.performed -= LeftAction;
        moveRightAction.action.performed -= RightAction;
        GameEvents.OnPlayerDeath -= DisableMovement;
        GameEvents.OnPauseStateChanged -= SwitchStateMovement;
    }

    private void RightAction(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        if (_currentLane == Lane.left) _currentLane = Lane.middle;
        else if (_currentLane == Lane.middle) _currentLane = Lane.right;
    }

    private void LeftAction(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        if (_currentLane == Lane.right) _currentLane = Lane.middle;
        else if (_currentLane == Lane.middle) _currentLane = Lane.left;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!_canMove) return;

        float targetXPosition = (int)_currentLane * laneDistance;
        float xDifference = targetXPosition - transform.position.x;
        _lateralVelocity = xDifference * snappingForce;

        var forward = _effects.CurrentSpeed * Time.deltaTime;
        var dodge = _lateralVelocity * Time.unscaledDeltaTime;
        Vector3 moveVector = new(dodge, 0f, forward);

        _controller.Move(moveVector);
    }

    private void DisableMovement() => _canMove = false;
    private void SwitchStateMovement(bool state)=>_canMove=!state;

}