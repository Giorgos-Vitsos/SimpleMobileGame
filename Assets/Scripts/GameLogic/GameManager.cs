using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyMultiplier = 1.2f;
    [SerializeField] private int tracksUntilDifficultyIncrease = 10;

    private int _score = 0;
    private PlayerEffects _playerEffects;
    private bool _gamePaused = false;
    private float _prevTimeScale = 1;
    private bool _firstTrack = true;

    private void Awake()
    {
        _playerEffects = FindAnyObjectByType<PlayerEffects>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += HandleGameOver;
        GameEvents.OnTrackCleared += HandleTrackCleared;
        GameEvents.OnRestartRequest += ReloadGame;
        GameEvents.OnPauseRequested += TogglePause;
        GameEvents.OnRestoreSaveData += RestoreData;
        GameEvents.OnGatherSaveData += InjectData;
        GameEvents.OnSaveRequest+=HandleSaveGame;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandleGameOver;
        GameEvents.OnTrackCleared -= HandleTrackCleared;
        GameEvents.OnRestartRequest -= ReloadGame;
        GameEvents.OnPauseRequested -= TogglePause;
        GameEvents.OnRestoreSaveData -= RestoreData;
        GameEvents.OnGatherSaveData -= InjectData;
        GameEvents.OnSaveRequest-=HandleSaveGame;
    }

    private void HandleTrackCleared()
    {
        if (_firstTrack)
        {
            _firstTrack = false;
            return;
        }
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

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            HandleLoadGame();
        }
    }

    private void HandleGameOver()
    {
        StartCoroutine(DeathSequence());
    }

    private void ReloadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void TogglePause()
    {
        _gamePaused = !_gamePaused;
        if (_gamePaused)
        {
            _prevTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = _prevTimeScale;
        }
        GameEvents.OnPauseStateChanged?.Invoke(_gamePaused);
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
    }

    private void HandleSaveGame()
    {
        GameStateData snapshot = new();
        GameEvents.OnGatherSaveData?.Invoke(snapshot);
        SaveManager.SaveGameState(snapshot);
    }

    private void HandleLoadGame()
    {
        GameStateData loadedData = SaveManager.LoadGameState();
        if (loadedData != null)
        {
            GameEvents.OnRestoreSaveData?.Invoke(loadedData);
        }
    }

    private void InjectData(GameStateData snapshot)
    {
        snapshot.currentScore = _score;
    }
    private void RestoreData(GameStateData data)
    {
        _score = data.currentScore;
        GameEvents.OnScoreUpdated?.Invoke(_score);
    }
}