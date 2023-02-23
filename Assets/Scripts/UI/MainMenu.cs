using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] GameController _script_GameController;

    private void Awake()
    {
        
    }


    public void PlayGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        //note the 2 means the 2 index of building list
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        Application.Quit();
    }
}
