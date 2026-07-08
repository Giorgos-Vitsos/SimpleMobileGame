using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicButton : MonoBehaviour
{
    [SerializeField] private Button actionButton; 
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private ButtonClicksManager manager;
    


    private bool _hasSaveFile;

    private void Awake()
    {

        _hasSaveFile = SaveManager.SaveExists();

        UpdateButtonStyle();
    }

    private void UpdateButtonStyle()
    {
        actionButton.onClick.RemoveAllListeners();
        if (_hasSaveFile)
        {
            buttonText.text = "Continue";
            actionButton.onClick.AddListener(manager.Click_Load);
        }
        else
        {
            buttonText.text = "Play";
            actionButton.onClick.AddListener(manager.Click_PlayGame);
        }
    }
}