using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleMapSetup : MonoBehaviour
{
    public GameObject scroll;
    public GameObject button;
    public GameObject deckButton;
    public GameObject deckButtonBackground;
    
    public void ShowBox()
    {
        scroll.SetActive(true);
        button.SetActive(false);
    }
    public void Start()
    {
        //Debug.Log("Battle Num" + BattleController.battleNum);
        if (BattleController.battleNum > 0 || GameController.instance.restartedTheGame)
        {
            deckButton.SetActive(true);
            deckButtonBackground.SetActive(true);
        }

        // Reset animations
        GameController.instance.UIAnimationsOffScreen();
        SoundManager.PlaySound("bgm_Mountain_Ambient", 0.15f);
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
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        // The first time at the map switch to the deck edit level
        if (BattleController.battleNum == 0 && GameController.instance.restartedTheGame == false)
        {
            SceneManager.LoadScene("DeckEditLevel");
        }
        else
        {
            SceneManager.LoadScene("BattleLevel");
        }
        
    }
    public void ChangeDecks()
    {
        SceneManager.LoadScene("PickDeckLevel_1");
    }

    public void PickDeck()
    {
        GameController.instance.DisableBattleMode(true);
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        SceneManager.LoadScene("DeckEditLevel");
    }

} 