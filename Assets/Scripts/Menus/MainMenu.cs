using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    GameController _script_GameController;
    //Chris trying to load fade animation, still figuring out
    //public Animator transition; 
    //IEnumerator FadeScreen()
    //{
    //    transition.SetTrigger("FadeIn");
    //    yield return new WaitForSeconds(1);
    //}

    private void Awake()
    {
        _script_GameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    private void Start()
    {
        SoundManager.PlaySound("bgm_Mountain_Ambient", 0.2f);
        //GameController.instance.EndDialogue();
    }

    public void PlayGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        SoundManager.bgmAudioSource.Stop();
        
        //fade animation plays 
        //animatorFadeScene.SetTrigger("FadeIn");
        //StartCoroutine(FadeScreen());
        SceneManager.LoadScene("StoryIntro");
    }

    public void PlayTutorial()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        SoundManager.bgmAudioSource.Stop();
        
        // --Legacy: Not used--
        //_script_GameController.isDeckELevel = true;
        //
        
        SceneManager.LoadScene("BattleMap");
    }
    
    public void CreditsScene()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        SoundManager.bgmAudioSource.Stop();
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        Application.Quit();
    }
}
