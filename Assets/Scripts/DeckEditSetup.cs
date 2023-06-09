using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditSetup : MonoBehaviour
{
    private void Start()
    {
        GameController.instance.ChangeBackground("Credits_BG");
        GameController.instance.CampSystemSetUp();
        
        DeckEditSystem.instance.RestartDeckSelecting();
        
        // Start with 3 cards to be selected
        DeckEditSystem.instance.SpawnCandidateForPick();
    }

    public void MapButton()
    {
        SceneManager.LoadScene("BattleMap");
    }
}
