using UnityEngine;

[RequireComponent(typeof(PlayerEffects))]
public class PlayerCollision : MonoBehaviour
{
    private PlayerEffects _effects;

    private void Awake()
    {
        _effects = GetComponent<PlayerEffects>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (_effects.IsInvincible)
            {
                other.gameObject.SetActive(false);
                return;
            }
            GameEvents.OnPlayerDeath?.Invoke(); 
        }
        else if (other.CompareTag("PowerUp"))
        {
            IPowerUps item = other.GetComponent<IPowerUps>();
            if (item != null)
            {
                item.ApplyEffect(_effects);
                other.gameObject.SetActive(false);
            }
        }
    }
}