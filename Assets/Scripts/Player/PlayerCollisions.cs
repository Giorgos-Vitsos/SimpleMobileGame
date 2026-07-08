using UnityEngine;

[RequireComponent(typeof(PlayerEffects))]
public class PlayerCollision : MonoBehaviour
{
    private PlayerEffects _effects;
    [SerializeField]private bool GodMode;

    private void Awake()
    {
        _effects = GetComponent<PlayerEffects>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GodMode)return;
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
                GameEvents.OnPlaySFX?.Invoke(SoundType.PickupPowerup);
                other.gameObject.SetActive(false);
            }
        }
    }
}