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

    private void Start()
    {
        if (_scoreText != null) _scoreText.text = "Score: 0";
        if (_scoreText != null) _finalScoreText.text = "Score: 0";
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += ShowDeathScreen;
        GameEvents.OnScoreUpdated += UpdateScoreDisplay;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= ShowDeathScreen;
        GameEvents.OnScoreUpdated -= UpdateScoreDisplay;
    }

    private void UpdateScoreDisplay(int newScore)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {newScore}";
        }
        if (_finalScoreText != null)
        {
            _finalScoreText.text = $"Score: {newScore}";
        }
    }

    private void ShowDeathScreen()
    {
        DisableGameUI();
        
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
        
        if (_mainUIFader != null)
        {
            _mainUIFader.TriggerFade(Fade.FadeType.Out);
        }
    }
}