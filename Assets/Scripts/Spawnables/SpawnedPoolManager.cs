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
    [SerializeField] private int initialMaxObstaclesPerTrack = 3;

    [Header("References")]
    [SerializeField] private SpawnedItem[] itemPrefabs;

    private Dictionary<Track, List<SpawnedItem>> _trackItems = new();
    private Dictionary<SpawnedItem, IObjectPool<SpawnedItem>> _objectPools = new();
    private List<SpawnedItem> _obstaclePrefabs = new();
    private List<SpawnedItem> _powerUpPrefabs = new();
    private Dictionary<string, SpawnedItem> _prefabLookup = new();

    private int _currMaxObstaclesPerTrack;

    private void Awake()
    {
        _currMaxObstaclesPerTrack = initialMaxObstaclesPerTrack;

        foreach (SpawnedItem prefab in itemPrefabs)
        {
            _prefabLookup[prefab.name] = prefab;
        }

        foreach (var prefab in itemPrefabs)
        {
            _objectPools[prefab] = new ObjectPool<SpawnedItem>(() => CreateItem(prefab), OnGet, OnRelease, OnDestroyItem, false, defaultCap, maxSize);

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

    private void OnEnable()
    {
        GameEvents.OnDifficultyIncreased += HandleDifficultySpike;
        GameEvents.OnGatherSaveData += InjectData;
        GameEvents.OnRestoreSaveData += RestoreData;
    }

    private void OnDisable()
    {
        GameEvents.OnDifficultyIncreased -= HandleDifficultySpike;
        GameEvents.OnGatherSaveData -= InjectData;
        GameEvents.OnRestoreSaveData -= RestoreData;
    }

    private void HandleDifficultySpike(int extraObstacles)
    {
        _currMaxObstaclesPerTrack += extraObstacles;
    }

    private void OnDestroyItem(SpawnedItem item) => Destroy(item.gameObject);
    private void OnRelease(SpawnedItem item) => item.OnDespawn();
    private void OnGet(SpawnedItem item) => item.OnSpawn();

    private SpawnedItem CreateItem(SpawnedItem prefab)
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

        Dictionary<Transform, int> obstaclesPerRow = new();

        foreach (Transform point in points)
        {
            SpawnedItem itemToSpawn = null;
            Transform rowParent = point.parent;

            if (!obstaclesPerRow.ContainsKey(rowParent))
            {
                obstaclesPerRow[rowParent] = 0;
            }

            if (obstacleCount < _currMaxObstaclesPerTrack && obstaclesPerRow[rowParent] < 2)
            {
                itemToSpawn = GetRandItem(SpawnedItem.ItemType.Obstacle);
                if (itemToSpawn != null)
                {
                    obstacleCount++;
                    obstaclesPerRow[rowParent]++;
                }
            }

            if (itemToSpawn == null)
            {
                if (powerUpCount < 1 && Random.value <= powerUpChance)
                {
                    itemToSpawn = GetRandItem(SpawnedItem.ItemType.Powerup);
                    if (itemToSpawn != null) powerUpCount++;
                }
                else
                {
                    continue;
                }
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

        if (candidates.Count == 0) return null;

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

    private void InjectData(GameStateData snapshot)
    {
        snapshot.currentMaxObstaclesPerTrack = _currMaxObstaclesPerTrack;
        foreach (KeyValuePair<Track, List<SpawnedItem>> entry in _trackItems)
        {
            Track track = entry.Key;
            List<SpawnedItem> items = entry.Value;

            if (items.Count == 0) continue;

            SavedTrackItems savedItems = new();
            savedItems.trackZPositionRounded = Mathf.RoundToInt(track.transform.position.z);

            foreach (SpawnedItem item in items)
            {

                string prefabName = item.PrefabSource.name;

                int spawnIndex = -1;
                for (int i = 0; i < track.spawnPoints.Length; i++)
                {
                    if ((track.spawnPoints[i].position - item.transform.position).sqrMagnitude < 0.01f)
                    {
                        spawnIndex = i;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(prefabName) && spawnIndex != -1)
                {
                    savedItems.itemPrefabNames.Add(prefabName);
                    savedItems.spawnPointIndices.Add(spawnIndex);
                }
            }

            snapshot.trackItems.Add(savedItems);
        }
    }

    private void RestoreData(GameStateData data)
    {
        _currMaxObstaclesPerTrack = data.currentMaxObstaclesPerTrack;
    }

    public void RestoreSpecificItems(Track track, SavedTrackItems savedItems)
    {
        List<SpawnedItem> restoredItems = new();

        for (int i = 0; i < savedItems.itemPrefabNames.Count; i++)
        {
            string prefabName = savedItems.itemPrefabNames[i];
            int spawnIndex = savedItems.spawnPointIndices[i];

            if (_prefabLookup.TryGetValue(prefabName, out SpawnedItem itemToSpawn))
            {
                SpawnedItem newItem = _objectPools[itemToSpawn].Get();
                newItem.transform.position = track.spawnPoints[spawnIndex].position;
                restoredItems.Add(newItem);
            }

            if (itemToSpawn != null)
            {
                SpawnedItem newItem = _objectPools[itemToSpawn].Get();
                newItem.transform.position = track.spawnPoints[spawnIndex].position;
                restoredItems.Add(newItem);
            }
        }

        _trackItems.Add(track, restoredItems);
    }
}