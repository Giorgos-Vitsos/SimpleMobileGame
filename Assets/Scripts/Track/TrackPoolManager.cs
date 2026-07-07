using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class TrackPoolManager : MonoBehaviour
{
    [Header("Segment Logic")]
    [SerializeField] private float TrackLength = 10f;

    [Header("Pool Management")]
    [SerializeField] private int defaultCap = 10;
    [SerializeField] private int maxSize = 100;

    [Header("References")]
    [SerializeField] private Track trackPrefab;
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private SpawnedPoolManager itemManager;

    private IObjectPool<Track> _trackPool;
    private Queue<Track> _activeTracks = new();
    private float _spawnPos = 0;

    private void Awake()
    {
        _trackPool = new ObjectPool<Track>(createTrack, OnGet, OnRelease, OnDestroyTrack, false, defaultCap, maxSize);
    }

    private void Start()
    {
        var isInitial=true;
        for (int i = 0; i < defaultCap; i++)
        {
            SpawnNextTrack(isInitial);
            isInitial=false;
        }
    }

    private void SpawnNextTrack(bool isInitial)
    {
        Track newTrack = _trackPool.Get();
        newTrack.transform.position = new Vector3(0, 0, _spawnPos);
        newTrack.SetupTrack();
        _spawnPos += TrackLength;
        _activeTracks.Enqueue(newTrack);
        
        if (!isInitial) itemManager.Populate(newTrack);
    }

    private void OnEnable()
    {
        GameEvents.OnGatherSaveData+=InjectData;
        GameEvents.OnRestoreSaveData+=RestoreData;
    }

    private void Disable()
    {
        GameEvents.OnGatherSaveData-=InjectData;
        GameEvents.OnRestoreSaveData-=RestoreData;
    }

    private void OnDestroyTrack(Track track) => Destroy(track.gameObject);
    private void OnRelease(Track track) => track.OnDespawn();
    private Track createTrack() => Instantiate(trackPrefab);
    private void OnGet(Track track) => track.OnSpawn();

    private void HandleTracks()
    {
        if (_activeTracks.Count == 0) return;
        
        Track oldestTrack = _activeTracks.Peek();
        
        
        if (oldestTrack.transform.position.z + TrackLength/2+2 < playerTransform.position.z)
        {
            itemManager.ClearItems(oldestTrack);
            _activeTracks.Dequeue();
            _trackPool.Release(oldestTrack);
            
            GameEvents.OnTrackCleared?.Invoke(); 
            
            SpawnNextTrack(false);
        }
    }

    private void Update() => HandleTracks();


    private void InjectData(GameStateData snapshot)
    {
        snapshot.nextSpawnPos=_spawnPos;
        foreach (Track track in _activeTracks)
        {
            snapshot.trackZPositionsRounded.Add(Mathf.RoundToInt(track.transform.position.z));
        }
    }

    private void RestoreData(GameStateData data)
    {
        while (_activeTracks.Count > 0)
        {
            Track oldTrack = _activeTracks.Dequeue();
            itemManager.ClearItems(oldTrack);
            _trackPool.Release(oldTrack);
        }

        _spawnPos = data.nextSpawnPos;

        foreach (int savedZRounded in data.trackZPositionsRounded)
        {
            Track loadedTrack = _trackPool.Get();
            loadedTrack.transform.position = new Vector3(0, 0, savedZRounded);
            loadedTrack.SetupTrack();
            _activeTracks.Enqueue(loadedTrack);

            SavedTrackItems savedItems = data.trackItems.Find(x => x.trackZPositionRounded == savedZRounded);
            
            if (savedItems != null)
            {
                itemManager.RestoreSpecificItems(loadedTrack, savedItems);
            }
        }
    }
}