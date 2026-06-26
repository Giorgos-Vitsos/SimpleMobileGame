using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runningSpeed=5f;
    [SerializeField] private float snappingForce=5f;
    [SerializeField] private float laneDistance=3f;
    

    [Header("References")]
    [SerializeField] private InputActionReference moveLeftAction;
    [SerializeField] private InputActionReference moveRightAction;


    private CharacterController _controller;
    private int _currentLane=0; //-1,0,1=left,middle,right



    void Awake()
    {
        _controller=GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        moveLeftAction.action.performed+=LeftAction;
        moveRightAction.action.performed+=RightAction;
        moveLeftAction.action.canceled+=LeftAction;
        moveRightAction.action.canceled+=RightAction;
    }

    void OnDisable()
    {
        moveLeftAction.action.performed-=LeftAction;
        moveRightAction.action.performed-=RightAction;
        moveLeftAction.action.canceled-=LeftAction;
        moveRightAction.action.canceled-=RightAction;
    }

    private void RightAction(InputAction.CallbackContext context)
    {
        if (_currentLane > -1)
        {
            _currentLane--;
        }
    }

    private void LeftAction(InputAction.CallbackContext context)
    {
        if (_currentLane < 1)
        {
            _currentLane++;
        }
    }

    void Update()
    {
        Debug.Log(_currentLane);
    }
}
