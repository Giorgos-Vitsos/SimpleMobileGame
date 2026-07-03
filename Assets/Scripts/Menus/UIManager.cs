using UnityEngine;
using TMPro; 

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
        if (active != null && active.Icon != null)
        {
            _activeIcon.sprite = active.Icon;
            _activeIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            _activeIcon.sprite = null;
            _activeIcon.color = new Color(1, 1, 1, 0);
        }

        if (queued != null && queued.Icon != null)
        {
            _queueIcon.sprite = queued.Icon;
            _queueIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            _queueIcon.sprite = null;
            _queueIcon.color = new Color(1, 1, 1, 0);
        }
    }
}