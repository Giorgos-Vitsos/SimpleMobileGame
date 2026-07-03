using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyMultiplier = 1.2f;
    [SerializeField] private int tracksUntilDifficultyIncrease = 10;

    private int _score = 0;
    private PlayerEffects _playerEffects; 

    private void Awake()
    {
        _playerEffects = FindAnyObjectByType<PlayerEffects>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += HandleGameOver;
        GameEvents.OnTrackCleared += HandleTrackCleared;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandleGameOver;
        GameEvents.OnTrackCleared -= HandleTrackCleared;
    }

    private void HandleTrackCleared()
    {
        _score++;
        
        GameEvents.OnScoreUpdated?.Invoke(_score);
        
        if (_score % tracksUntilDifficultyIncrease == 0 && _score != 0)
        {
            if (_playerEffects != null)
            {
                _playerEffects.MultiplySpeed(difficultyMultiplier);
            }
            GameEvents.OnDifficultyIncreased?.Invoke(1);
        }
    }

    private void HandleGameOver()
    {
        Time.timeScale = 0f;
    }
}