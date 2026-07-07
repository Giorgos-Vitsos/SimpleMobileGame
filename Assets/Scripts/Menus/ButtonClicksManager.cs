using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class ButtonClicksManager : MonoBehaviour
{
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

    public void Click_Save()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameEvents.OnSaveRequest?.Invoke();
    }

    public void Click_MainMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale=1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Click_Quit()
    {
        Debug.Log("Game is quitting!");
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
    }
}
