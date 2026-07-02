using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using NUnit.Framework;
public class TrackPoolManager : MonoBehaviour
{

    [Header("Segment Logic")]
    [SerializeField] private float TrackLength = 10f;

    [Header("Pool Managment")]
    [SerializeField] private int defaultCap = 10;
    [SerializeField] private int maxSize = 100;

    [Header("References")]
    [SerializeField] private Track trackPrefab;
    [SerializeField] private CharController player;
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
        for (int i = 0; i < defaultCap; i++)
        {
            SpawnNextTrack(true);
        }
    }

    

    private void SpawnNextTrack(bool isInitial)
    {
        Track newTrack = _trackPool.Get();
        newTrack.transform.position = new Vector3(0, 0, _spawnPos);
        _spawnPos += TrackLength;
        _activeTracks.Enqueue(newTrack);
        if (!isInitial)
        {
            itemManager.Populate(newTrack);
        }
        
    }

    private void OnDestroyTrack(Track track)
    {
        Destroy(track.gameObject);

    }

    private void OnRelease(Track track)
    {
        track.OnDespawn();
    }

    private Track createTrack()
    {
        return Instantiate(trackPrefab);
    }

    private void OnGet(Track track)
    {
        track.OnSpawn();
    }

    private void HandleTracks()
    {
        if (_activeTracks.Count == 0)
        {
            return;
        }
        Track oldestTrack = _activeTracks.Peek();
        if (oldestTrack.transform.position.z + TrackLength < player.transform.position.z)
        {
            itemManager.ClearItems(oldestTrack);
            _activeTracks.Dequeue();
            _trackPool.Release(oldestTrack);
            player.UpScore();
            
            SpawnNextTrack(false);
        }

    }

    private void Update()
    {
        HandleTracks();

    }
}
