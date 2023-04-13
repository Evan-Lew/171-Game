using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameController _script_GameController;

    private void Awake()
    {
        _script_GameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    private void Start()
    {
        SoundManager.PlaySound("bgm_Mountain_Ambient", 0.2f);
    }

    public void PlayGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        SoundManager.bgmAudioSource.Stop();
        
        SceneManager.LoadScene("DeveloperLevel");
    }

    public void PlayTutorial()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        SoundManager.bgmAudioSource.Stop();
        
        // --Legacy: Not used--
        _script_GameController.isDeckELevel = true;
        //
        
        SceneManager.LoadScene("TutorialLevel");
    }
    
    public void CreditsScene()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        SoundManager.bgmAudioSource.Stop();
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        Application.Quit();
    }
}
