using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Death Screen UI (Game Over)")]
    [SerializeField] private Fade deathScreenFader;
    [SerializeField] private CanvasGroup deathScreenCanvasGroup;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Pause Screen UI (Paused)")]
    [SerializeField] private Fade pauseScreenFader;
    [SerializeField] private CanvasGroup pauseScreenCanvasGroup;
    [SerializeField] private TextMeshProUGUI pauseScoreText;

    [Header("Gameplay UI (Main UI)")]
    [SerializeField] private Fade mainUIFader;
    [SerializeField] private CanvasGroup mainUICanvasGroup;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("PowerUp HUD")]
    [SerializeField] private UnityEngine.UI.Image activeIcon;
    [SerializeField] private UnityEngine.UI.Image queueIcon;

    private int _cachedScore = 0;
    private bool _isFlashing = false;

    private void Start()
    {
        if (scoreText != null) scoreText.text = "Score: 0";
        if (finalScoreText != null) finalScoreText.text = "Score: 0";
    }

    private void Update()
    {
        HandleIconFlashing();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += ShowDeathScreen;
        GameEvents.OnScoreUpdated += UpdateScoreDisplay;
        GameEvents.OnEffectsHUDUpdated += UpdateEffectsHUD;
        GameEvents.OnPauseStateChanged+=HandlePauseMenu;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= ShowDeathScreen;
        GameEvents.OnScoreUpdated -= UpdateScoreDisplay;
        GameEvents.OnEffectsHUDUpdated -= UpdateEffectsHUD;
        GameEvents.OnPauseStateChanged-=HandlePauseMenu;
    }

    private void UpdateScoreDisplay(int newScore)
    {
        _cachedScore = newScore;

        if (scoreText != null)
        {
            scoreText.text = $"Score: {_cachedScore}";
        }
    }

    private void ShowDeathScreen()
    {
        DisableGameUI();

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {_cachedScore}";
        }

        if (deathScreenCanvasGroup != null)
        {
            deathScreenCanvasGroup.interactable = true;
            deathScreenCanvasGroup.blocksRaycasts = true;
        }

        if (deathScreenFader != null)
        {
            deathScreenFader.TriggerFade(Fade.FadeType.In);
        }
    }

    private void ShowPauseScreen()
    {
        DisableGameUI();

        if (pauseScoreText != null)
        {
            pauseScoreText.text = $"Score: {_cachedScore}";
        }

        if (pauseScreenCanvasGroup != null)
        {
            pauseScreenCanvasGroup.interactable = true;
            pauseScreenCanvasGroup.blocksRaycasts = true;
        }

        if (pauseScreenFader != null)
        {
            pauseScreenFader.TriggerFade(Fade.FadeType.In);
        }
    }

    private void HidePauseScreen()
    {
        DisableGameUI();

        if (pauseScreenCanvasGroup != null)
        {
            pauseScreenCanvasGroup.interactable = false;
            pauseScreenCanvasGroup.blocksRaycasts = false;
        }

        if (pauseScreenFader != null)
        {
            pauseScreenFader.TriggerFade(Fade.FadeType.Out);
        }
        EnableGameUI();
    }

    private void DisableGameUI()
    {
        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.interactable = false;
            mainUICanvasGroup.blocksRaycasts = false;
        }

        if (mainUIFader != null)
        {
            mainUIFader.TriggerFade(Fade.FadeType.Out);
        }
    }

    private void EnableGameUI()
    {
        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.interactable = true;
            mainUICanvasGroup.blocksRaycasts = true;
        }

        if (mainUIFader != null)
        {
            mainUIFader.TriggerFade(Fade.FadeType.In);
        }
    }

    private void UpdateEffectsHUD(Sprite activeSprite, Sprite queuedSprite, bool isWarning)
    {
        _isFlashing = isWarning;
        if (activeSprite != null)
        {
            activeIcon.sprite = activeSprite;
        }
        else
        {
            activeIcon.sprite = null;
            activeIcon.color = new Color(1, 1, 1, 0);
        }

        if (queuedSprite != null)
        {
            queueIcon.sprite = queuedSprite;
            queueIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            queueIcon.sprite = null;
            queueIcon.color = new Color(1, 1, 1, 0);
        }
    }

    

    private void HandleIconFlashing()
    {
        if (_isFlashing && activeIcon.sprite != null)
        {
            float wave = (Mathf.Sin(Time.unscaledTime * 15f) + 1f) / 2f;
            float alpha = Mathf.Lerp(0.2f, 1f, wave);
            activeIcon.color = new Color(1, 1, 1, alpha);
        }
        else if (!_isFlashing && activeIcon.sprite != null)
        {
            activeIcon.color = new Color(1, 1, 1, 1);
        }
    }

    private void HandlePauseMenu(bool state)
    {
        if (state)
        {
            ShowPauseScreen();
        }
        else
        {
            HidePauseScreen();
        }
    }
}