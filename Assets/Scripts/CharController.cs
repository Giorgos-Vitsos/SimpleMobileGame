using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float snappingForce = 5f;
    [SerializeField] private float laneDistance = 4.5f;


    [Header("References")]
    [SerializeField] private InputActionReference moveLeftAction;
    [SerializeField] private InputActionReference moveRightAction;


    private CharacterController _controller;
    private Lane _currentLane = Lane.middle;
    private float _lateralVelocity;

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
                _currentLane=Lane.right;
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
                _currentLane=Lane.left;
                break;
        }
    }

    void Update()
    {

        Debug.Log(_currentLane);
        CalculateLaneVelocity();
        ApplyMovement();
    }

    private void CalculateLaneVelocity()
    {
        float targetXPosition = (int)_currentLane * laneDistance;
        float xDifference = targetXPosition - transform.position.x;
        _lateralVelocity = xDifference * snappingForce;
    }

    private void ApplyMovement()
    {
        Vector3 moveVector = new Vector3(_lateralVelocity, 0f, runningSpeed);
        _controller.Move(moveVector * Time.unscaledDeltaTime);
    }
}
