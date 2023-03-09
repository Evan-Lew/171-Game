using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameController _script_GameController;

    private void Awake()
    {
        _script_GameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }


    public void PlayGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        //note the 2 means the 2 index of building list
        _script_GameController.isDeckELevel = true;
        SceneManager.LoadScene(2);
    }

    public void CreditsGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        Application.Quit();
    }
}
