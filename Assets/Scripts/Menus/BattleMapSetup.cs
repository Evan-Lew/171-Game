using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleMapSetup : MonoBehaviour
{
    public GameObject scroll;
    public GameObject button;
    public GameObject deckbutton;
    
    public void ShowBox()
    {
        scroll.SetActive(true);
        button.SetActive(false);
    }
    public void Start()
    {
        // Reset animations
        GameController.instance.UIAnimationsOffScreen();
        SoundManager.PlaySound("bgm_Mountain_Ambient", 0.5f);
        deckbutton.SetActive(false);
    }
    public void Update()
    {
        // if (BattleController.battleNum > 0)
        // {
        //     button.SetActive(false);
        //     deckbutton.SetActive(true);
        // }
    }

    public void ToMap()
    {
        SceneManager.LoadScene("BattleMap");
    }

    public void StartBattle()
    {

        SceneManager.LoadScene("BattleLevel");
    }
    public void ChangeDecks()
    {
        SceneManager.LoadScene("PickDeckLevel_1");
    }

} 