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
    private bool _canMove = true;//pause or death

    public enum Lane { left = -1, middle = 0, right = 1 }

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
        GameEvents.OnPauseStateChanged += SwitchStateMovement;
        GameEvents.OnGatherSaveData += InjectData;
        GameEvents.OnRestoreSaveData += RestoreData;
    }

    private void OnDisable()
    {
        moveLeftAction.action.performed -= LeftAction;
        moveRightAction.action.performed -= RightAction;
        GameEvents.OnPlayerDeath -= DisableMovement;
        GameEvents.OnPauseStateChanged -= SwitchStateMovement;
        GameEvents.OnGatherSaveData -= InjectData;
        GameEvents.OnRestoreSaveData -= RestoreData;
    }

    private void RightAction(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        if (_currentLane == Lane.left) _currentLane = Lane.middle;
        else if (_currentLane == Lane.middle) _currentLane = Lane.right;
        GameEvents.OnPlayerDodge?.Invoke(1);
    }

    private void LeftAction(InputAction.CallbackContext context)
    {
        if (!_canMove) return;
        if (_currentLane == Lane.right) _currentLane = Lane.middle;
        else if (_currentLane == Lane.middle) _currentLane = Lane.left;
        GameEvents.OnPlayerDodge?.Invoke(-1);
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
        float _lateralVelocity = xDifference * snappingForce;

        _controller.Move(new Vector3(_lateralVelocity, 0f, _effects.CurrentSpeed) * Time.deltaTime);
    }

    private void DisableMovement() => _canMove = false;
    private void SwitchStateMovement(bool state) => _canMove = !state;

    private void InjectData(GameStateData snapshot)
    {
        snapshot.currentLane = _currentLane;
        snapshot.playerZPosition = transform.position.z;
    }

    private void RestoreData(GameStateData data)
    {
        _currentLane = data.currentLane;
        _controller.enabled = false;
        transform.position = new Vector3(0, 0, data.playerZPosition);
        _controller.enabled = true;
    }

}