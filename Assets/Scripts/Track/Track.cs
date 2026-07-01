using UnityEngine;


public class Track : MonoBehaviour
{
    [Header("Spawn Locations")]
    [SerializeField]private SpawnedItem itemPrefab;
    [SerializeField] public Transform[] spawnPoints;
   public void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }
}
