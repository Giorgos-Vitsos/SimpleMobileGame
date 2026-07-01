using UnityEngine;



public class SpawnedItem : MonoBehaviour
{
    [SerializeField] private ItemType type;
    public enum ItemType { Obstacle, Powerup }
    
    public ItemType Type=>type;
    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        gameObject.SetActive(false);
    }
    
    public SpawnedItem PrefabSource { get; set; }

}
