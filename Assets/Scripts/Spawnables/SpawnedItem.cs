using UnityEngine;


public class SpawnedItem : MonoBehaviour
{
    [Header("Item type")]
    [SerializeField] private ItemType type;
    public enum ItemType { Obstacle, Powerup }
    public SpawnedItem PrefabSource { get; set; }

    public ItemType Type => type;
    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        gameObject.SetActive(false);
    }

}
