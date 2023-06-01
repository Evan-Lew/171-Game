using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditSetup : MonoBehaviour
{
    private void Start()
    {
        GameController.instance.ChangeBackground("Cave_BG");
        GameController.instance.CampSystemSetUp();
    }

    public void MapButton()
    {
        SceneManager.LoadScene("BattleMap");
    }
}
