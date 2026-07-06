using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private PlayerEffects _playerEffects;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerEffects = GetComponentInParent<PlayerEffects>(); 

    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += TriggerDeath;
        GameEvents.OnPlayerDodge += TriggerDodge;

    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= TriggerDeath;
        GameEvents.OnPlayerDodge -= TriggerDodge;
        
    }

    private void Update()
    {
        HandleSpeed();
    }
    
    private void HandleSpeed()
    {
        if (_playerEffects != null)
        {
            Debug.Log($"Speed for locomotion is: {_playerEffects.CurrentSpeed}");
            _animator.SetFloat("Speed",_playerEffects.CurrentSpeed);
        }
    }

    private void TriggerDeath() => _animator.SetTrigger("Die");

    private void TriggerDodge(int direction)
    {
        if (direction == -1)
        {
            _animator.SetTrigger("TurnL");
        }else if (direction == 1)
        {
            _animator.SetTrigger("TurnR");
        }
    }

}
