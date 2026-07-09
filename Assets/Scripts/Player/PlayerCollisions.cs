using UnityEngine;

[RequireComponent(typeof(PlayerEffects))]
public class PlayerCollision : MonoBehaviour
{

    [Header("Testing")]
    [SerializeField] private bool GodMode;//for testing
    private PlayerEffects _effects;

    private void Awake()
    {
        _effects = GetComponent<PlayerEffects>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GodMode) return;
        if (other.CompareTag("Obstacle"))
        {
            if (_effects.IsInvincible)//if we have shield
            {
                GameEvents.OnPlaySFX?.Invoke(SoundType.ObstacleBreak);//destroy obstacle
                other.gameObject.SetActive(false);
                return;
            }
            GameEvents.OnPlayerDeath?.Invoke();//else die
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