using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class SpawnedPoolManager : MonoBehaviour
{

    [Header("Spawn Settings")]
    [SerializeField] private int maxSize = 50;
    [SerializeField] private int defaultCap = 5;
    [SerializeField] private float powerUpChance = 0.1f;

    [Header("Difficulty Limits")]
    [SerializeField] private int maxObstaclesPerTrack = 5;
    [SerializeField] private int maxPowerUpsPerTrack = 1;

    [Header("References")]
    [SerializeField] private SpawnedItem[] itemPrefabs;

    private Dictionary<Track, List<SpawnedItem>> _trackItems = new();
    private Dictionary<SpawnedItem, IObjectPool<SpawnedItem>> _objectPools = new();
    private List<SpawnedItem> _obstaclePrefabs = new();
    private List<SpawnedItem> _powerUpPrefabs = new();


    private void Awake()
    {
        foreach (var prefab in itemPrefabs)
        {
            _objectPools[prefab] = new ObjectPool<SpawnedItem>(() => createItem(prefab), OnGet, OnRelease, OnDestroyItem, false, defaultCap, maxSize);
            if (prefab.Type == SpawnedItem.ItemType.Obstacle)
            {
                _obstaclePrefabs.Add(prefab);
            }
            else if (prefab.Type == SpawnedItem.ItemType.Powerup)
            {
                _powerUpPrefabs.Add(prefab);
            }
        }
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

    private SpawnedItem createItem(SpawnedItem prefab)
    {
        SpawnedItem item = Instantiate(prefab);
        item.PrefabSource = prefab;
        item.OnDespawn();
        return item;
    }

    public void Populate(Track track)
    {
        var obstacleCount = 0;
        var powerUpCount = 0;
        List<SpawnedItem> currItems = new();
        List<Transform> points = new(track.spawnPoints);
        ShuffleUtility.Shuffle(points);

        foreach (Transform point in points)
        {

            SpawnedItem itemToSpawn = null;

            if (Random.value > powerUpChance && obstacleCount < 2)
            {
                itemToSpawn = GetRandItem(SpawnedItem.ItemType.Obstacle);
                if (itemToSpawn != null)
                {
                    obstacleCount++;
                }

            }
            else if (Random.value <= powerUpChance && powerUpCount < 1)
            {
                itemToSpawn = GetRandItem(SpawnedItem.ItemType.Powerup);
                if (itemToSpawn != null)
                {
                    powerUpCount++;
                }
            }
            if (itemToSpawn == null)
            {
                continue;
            }
            SpawnedItem newItem = _objectPools[itemToSpawn].Get();
            newItem.transform.position = point.position;
            currItems.Add(newItem);
        }
        _trackItems.Add(track, currItems);
    }

    private SpawnedItem GetRandItem(SpawnedItem.ItemType itemType)
    {
        List<SpawnedItem> candidates = (itemType == SpawnedItem.ItemType.Obstacle) ? _obstaclePrefabs : _powerUpPrefabs;
        if (candidates.Count == 0)
        {
            return null;
        }
        return candidates[Random.Range(0, candidates.Count)];
    }
    public void ClearItems(Track track)
    {
        if (_trackItems.ContainsKey(track))
        {
            foreach (var item in _trackItems[track])
            {
                _objectPools[item.PrefabSource].Release(item);
            }
            _trackItems.Remove(track);
        }
    }
}
