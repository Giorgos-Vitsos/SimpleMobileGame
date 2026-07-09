using UnityEngine;

public class Track : MonoBehaviour
{
    [Header("Spawn Points Settings")]
    [SerializeField] private SpawnedItem itemPrefab;
    [SerializeField] public Transform[] spawnPoints;
    public void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    //setups the mileston sign
    public void SetupTrack()
    {
        GetComponentInChildren<MileStonSign>().UpdateDistance(transform.position.z);
    }
}
