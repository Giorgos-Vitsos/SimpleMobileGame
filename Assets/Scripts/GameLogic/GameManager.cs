using UnityEngine;
using UnityEngine.SceneManagement;

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
        GameEvents.OnRestartRequest+=ReloadGame;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandleGameOver;
        GameEvents.OnTrackCleared -= HandleTrackCleared;
        GameEvents.OnRestartRequest-=ReloadGame;
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

    private void ReloadGame()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}