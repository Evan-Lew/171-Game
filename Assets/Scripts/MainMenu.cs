using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameController _script_GameController;

    private void Awake()
    {
        
    }


    public void PlayGame()
    {
        _script_GameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        //ask for setup combat system
        //_script_GameController.setupFlag = true;
        SceneManager.LoadScene("Template");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
