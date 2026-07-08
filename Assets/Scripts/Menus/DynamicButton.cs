using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicButton : MonoBehaviour
{
    [SerializeField] private Button actionButton;
    [SerializeField] private Button deleteButton; 
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private ButtonClicksManager manager;
    
    private bool _hasSaveFile;

    private void OnEnable()
    {
        GameEvents.OnDeleteRequest+= ResetButtons;
    }

    private void OnDisable()
    {
        GameEvents.OnDeleteRequest-= ResetButtons;
    }

    private void Awake()
    {

        _hasSaveFile = SaveManager.SaveExists();

        UpdateButton();
    }

    private void UpdateButton()
    {
        actionButton.onClick.RemoveAllListeners();
        if (_hasSaveFile)
        {
            deleteButton.gameObject.SetActive(true);
            buttonText.text = "Continue";
            actionButton.onClick.AddListener(manager.Click_Load);
        }
        else
        {
            deleteButton.gameObject.SetActive(false);
            buttonText.text = "Play";
            actionButton.onClick.AddListener(manager.Click_PlayGame);
        }
    }

    private void ResetButtons()
    {
        _hasSaveFile=false;
        UpdateButton();
    }


}