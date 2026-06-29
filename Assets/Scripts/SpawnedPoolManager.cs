using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class SpawnedPoolManager : MonoBehaviour
{
    [SerializeField]private SpawnedItem itemPrefab;
    [SerializeField]private int maxSize=100;
    [SerializeField]private int defaultCap=30;
    [SerializeField]private float objSpawnChance=0.5f;

    private IObjectPool<SpawnedItem> objectPool;
    private Dictionary<Track , List<SpawnedItem>> _trackItems=new Dictionary<Track, List<SpawnedItem>>();
    private void Awake()
    {
        objectPool=new ObjectPool<SpawnedItem>(createItem,OnGet,OnRelease,OnDestroyItem,false,defaultCap,maxSize);
    }

    private void OnDestroyItem(SpawnedItem item)
    {
        Destroy(item.gameObject);
    }

    private void OnRelease(SpawnedItem item)
    {
        item.OnDespawn();
    }

    private void OnGet(SpawnedItem item)
    {
        item.OnSpawn();
    }

    private SpawnedItem createItem()
    {
        return Instantiate(itemPrefab);
    }

    public void Populate(Track track)
    {
        var spawned=0;
        List<SpawnedItem> currItems=new List<SpawnedItem>();
        List<Transform> points=new List<Transform>(track.spawnPoints);
        ShuffleUtility.Shuffle(points);
        foreach(Transform point in points)
        {
            if (Random.value <= objSpawnChance && spawned<2)
            {
                SpawnedItem newItem=objectPool.Get();
                newItem.transform.position=point.position;
                currItems.Add(newItem);
                spawned++;
            }
        }
        _trackItems.Add(track,currItems);
    }

    public void ClearItems(Track track)
    {
        if (_trackItems.ContainsKey(track))
        {
            foreach(SpawnedItem item in _trackItems[track])
            {
                objectPool.Release(item);
            }
            _trackItems.Remove(track);
        }
    }
}
