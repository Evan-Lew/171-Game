using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameController _script_GameController;

    private void Awake()
    {
        _script_GameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }


    public void PlayGame()
    {
        //ask for setup combat system
        _script_GameController.setupFlag = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
