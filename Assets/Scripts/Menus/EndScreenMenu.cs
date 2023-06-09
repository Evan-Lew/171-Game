using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenMenu : MonoBehaviour
{
    public GameObject endText1;
    public GameObject endText2;
    public GameObject endText3;
    public GameObject gameOverText;

    public GameObject returnButton;
    public GameObject nextButton;
    public GameObject quitButton;
    
    private int _textNumber = 1;
    private void Start()
    {
        SoundManager.PlaySound("bgm_Finale", 0.25f);
        
        // If the player beat the game
        if (GameController.instance.beatTheGame)
        {
            endText1.SetActive(true);
            gameOverText.SetActive(false);
            returnButton.SetActive(false);
        }
        else if (GameController.instance.beatTheGame == false)
        {
            gameOverText.SetActive(true);
            nextButton.SetActive(false);
            quitButton.SetActive(true);
        }
    }

    public void NextButton()
    {
        _textNumber += 1;
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        if (_textNumber == 2)
        {
            endText1.SetActive(false);
            endText2.SetActive(true);
        }
        else if (_textNumber == 3)
        {
            endText2.SetActive(false);
            endText3.SetActive(true);
            nextButton.SetActive(false);
            quitButton.SetActive(true);
        }
    }

    public void ReturnButton()
    {
        GameController.instance.restartedTheGame = true;
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        SceneManager.LoadScene("BattleMap");
    }
    
    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        Application.Quit();
    }
}
