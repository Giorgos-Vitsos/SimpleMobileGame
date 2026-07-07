using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Death Screen UI (Game Over)")]
    [SerializeField] private Fade _deathScreenFader;
    [SerializeField] private CanvasGroup _deathScreenCanvasGroup;
    [SerializeField] private TextMeshProUGUI _finalScoreText;

    [Header("Gameplay UI (Main UI)")]
    [SerializeField] private Fade _mainUIFader;
    [SerializeField] private CanvasGroup _mainUICanvasGroup;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("PowerUp HUD")]
    [SerializeField] private UnityEngine.UI.Image _activeIcon;
    [SerializeField] private UnityEngine.UI.Image _queueIcon;

    [Header("PowerUp Fallback Icons (For Loaded Saves)")]
    [SerializeField] private Sprite _shieldFallbackIcon;
    [SerializeField] private Sprite _slowMoFallbackIcon;
    [SerializeField] private Sprite _speedFallbackIcon;
    [SerializeField] private Sprite _combinedFallbackIcon; 

    private int _cachedScore = 0;

    private void Start()
    {
        if (_scoreText != null) _scoreText.text = "Score: 0";
        if (_finalScoreText != null) _finalScoreText.text = "Score: 0";
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += ShowDeathScreen;
        GameEvents.OnScoreUpdated += UpdateScoreDisplay;
        GameEvents.OnEffectsHUDUpdated += UpdateEffectsHUD;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= ShowDeathScreen;
        GameEvents.OnScoreUpdated -= UpdateScoreDisplay;
        GameEvents.OnEffectsHUDUpdated -= UpdateEffectsHUD;
    }

    private void UpdateScoreDisplay(int newScore)
    {
        _cachedScore = newScore;
        
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {_cachedScore}";
        }
    }

    private void ShowDeathScreen()
    {
        DisableGameUI();
        
        if (_finalScoreText != null)
        {
            _finalScoreText.text = $"Score: {_cachedScore}";
        }
        
        if (_deathScreenCanvasGroup != null)
        {
            _deathScreenCanvasGroup.interactable = true;
            _deathScreenCanvasGroup.blocksRaycasts = true;
        }
        
        if (_deathScreenFader != null)
        {
            _deathScreenFader.TriggerFade(Fade.FadeType.In);
        }
    }

    private void DisableGameUI()
    {
        if (_mainUICanvasGroup != null)
        {
            _mainUICanvasGroup.interactable = false;
            _mainUICanvasGroup.blocksRaycasts = false;
        }

        if (_mainUIFader != null)
        {
            _mainUIFader.TriggerFade(Fade.FadeType.Out);
        }
    }

    private void UpdateEffectsHUD(StatusEffect active, StatusEffect queued)
    {
        Sprite activeSprite = GetIconForEffect(active);
        if (activeSprite != null)
        {
            _activeIcon.sprite = activeSprite;
            _activeIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            _activeIcon.sprite = null;
            _activeIcon.color = new Color(1, 1, 1, 0);
        }

        Sprite queueSprite = GetIconForEffect(queued);
        if (queueSprite != null)
        {
            _queueIcon.sprite = queueSprite;
            _queueIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            _queueIcon.sprite = null;
            _queueIcon.color = new Color(1, 1, 1, 0);
        }
    }

    private Sprite GetIconForEffect(StatusEffect effect)
    {
        if (effect == null) return null;

   
        if (effect.Icon != null) return effect.Icon;

        if (effect is ShieldEffect) return _shieldFallbackIcon;
        if (effect is SlowMoEffect) return _slowMoFallbackIcon;
        if (effect is SpeedEffect) return _speedFallbackIcon;
        if (effect is CombinedEffect) return _combinedFallbackIcon;

        return null;
    }

    public void Click_PlayGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("MainGame");
    }

    public void Click_Restart()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameEvents.OnRestartRequest?.Invoke();
    }

    public void Click_Pause()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameEvents.OnPauseRequested?.Invoke();
    }

    public void Click_Quit()
    {
        Debug.Log("Game is quitting!");
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
    }
}