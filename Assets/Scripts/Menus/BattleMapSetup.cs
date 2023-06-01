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
        Debug.Log("Battle Num" + BattleController.battleNum);
        // Reset animations
        GameController.instance.UIAnimationsOffScreen();
        
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
        // The first time at the map switch to the deck edit level
        if (BattleController.battleNum == 0)
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
        SceneManager.LoadScene("DeckEditLevel");
    }

} 