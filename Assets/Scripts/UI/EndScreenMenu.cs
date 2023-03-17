using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenMenu : MonoBehaviour
{
    private void Start()
    {
        //SoundManager.PlaySound("bgm_Mountain_Ambient", 0.2f);
    }
    
    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        Application.Quit();
    }
}
