using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        Time.timeScale = 1f;
        
        pauseMenuUI.SetActive(false);
        GameController.instance.DisableBattleMode(true);
        // Load main menu
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        Application.Quit();
    }
}
