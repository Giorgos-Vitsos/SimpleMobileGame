using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Start()
    {
        if (PlayerPrefs.GetInt("ShouldLoadSave", 0) == 1)
        {
            Debug.Log("BIKA");
            PlayerPrefs.SetInt("ShouldLoadSave", 0);
            GameEvents.OnLoadRequest?.Invoke();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += HandleGameOver;
        GameEvents.OnTrackCleared += HandleTrackCleared;
        GameEvents.OnRestartRequest += ReloadGame;
        GameEvents.OnPauseRequested += TogglePause;
        GameEvents.OnRestoreSaveData += RestoreData;
        GameEvents.OnGatherSaveData += InjectData;
        GameEvents.OnSaveRequest += HandleSaveGame;
        GameEvents.OnLoadRequest += HandleLoadGame;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandleGameOver;
        GameEvents.OnTrackCleared -= HandleTrackCleared;
        GameEvents.OnRestartRequest -= ReloadGame;
        GameEvents.OnPauseRequested -= TogglePause;
        GameEvents.OnRestoreSaveData -= RestoreData;
        GameEvents.OnGatherSaveData -= InjectData;
        GameEvents.OnSaveRequest -= HandleSaveGame;
        GameEvents.OnLoadRequest -= HandleLoadGame;
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
        snapshot.firstTrack=_firstTrack;
    }
    private void RestoreData(GameStateData data)
    {
        _score = data.currentScore;
        _firstTrack=data.firstTrack;
        GameEvents.OnScoreUpdated?.Invoke(_score);
    }

}