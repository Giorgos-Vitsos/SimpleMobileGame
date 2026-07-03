using UnityEngine;
using TMPro; // Assuming you use TextMeshPro for the score

public class UIManager : MonoBehaviour
{
    [Header("Death Screen UI")]
    [SerializeField] private Fade _deathScreenFader;
    [SerializeField] private CanvasGroup _deathScreenCanvasGroup;

    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        if (_scoreText != null) _scoreText.text = "Score: 0";
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
    }

    private void ShowDeathScreen()
    {
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
}