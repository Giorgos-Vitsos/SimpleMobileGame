using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Click_PlayGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void Click_Quit()
    {
        Debug.Log("Game is quitting!");
        Application.Quit();
    }
}
