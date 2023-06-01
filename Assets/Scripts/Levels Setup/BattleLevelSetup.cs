using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleLevelSetup : MonoBehaviour
{
    DeckSystem _deckSystem;
    HandManager _handManager;

    Character_Basedata enemy;

    [Header("List of Cards to start the deck with")]
    [SerializeField] List<Card_Basedata> startingCards;

    [SerializeField] Character_Basedata[] listOfEnemies;

    private bool _levelEnd = false;
    
    [SerializeField] String backgroundName;
    [SerializeField] List<String> backgroundsList;

    private void Start()
    {
        // Deactivate all story background game objects (just to make sure they're deactivated)
        for (int i = 0; i < GameController.instance.storyBackgroundsList.Count; i++)
        {
            GameController.instance.storyBackgroundsList[i].SetActive(false);
        }
        
        GameController.instance.EndDialogue("BattleLevel");
        //GameController.instance.FadeIn();
        
        // Change background
        GameController.instance.ChangeBackground(backgroundsList[BattleController.battleNum]);
        // Change sprite to Bai She Zhuan
        GameController.instance.changePlayerSprite();
        
        _deckSystem = GameObject.Find("Deck System").GetComponent<DeckSystem>();
        _handManager = GameObject.Find("Hand System").GetComponent<HandManager>();
        StartTheBattle();
    }

    void Update()
    {
        LevelManagement();
    }

    void StartTheBattle()
    {
        // Resetting the hand draw
        _deckSystem.enableDrawing = true;
        _deckSystem.deckToUse.Clear();
        _deckSystem.deckForCurrentBattle.Clear();
        _handManager.Clear();
        
        // Add the cards specified in the list to the deck to be used
        foreach (Card_Basedata card in startingCards)
        {
            _deckSystem.deckToUse.Add(card);
            _deckSystem.deckForCurrentBattle.Add(card);
        }
        _deckSystem.DrawMultipleCards(5);

        // listOfEnemies
        // 0, 1, 2 == fodder enemies
        // 3 == elite enemy
        // 99 == boss

        // if (BattleController.battleNum == 99)
        // {
        //     enemy = listOfEnemies.Where(obj => obj.characterName == "Stone Rui Shi").SingleOrDefault();
        // } else {
        //     enemy = listOfEnemies.Where(obj => obj.characterName == listOfEnemies[BattleController.battleNum].characterName).SingleOrDefault();
        // }
        
        GameController.instance.StartTheBattle(listOfEnemies[BattleController.battleNum], true);
        GameController.instance.battleCondition = true;
        BattleController.instance.player.Health_Current = BattleController.end_HP;
    }

    void LevelManagement()
    {
        // Player wins
        if (BattleController.instance.enemy.Health_Current <= 0 && _levelEnd == false)
        {
            BattleController.instance.enableTurnUpdate = false;
            
            BattleController.battleNum++;
            BattleController.totalLevel++;
            _levelEnd = true;
        }
        
        if (_levelEnd)
        {
            BattleController.end_HP = BattleController.instance.player.Health_Current;
            GameController.instance.DisableBattleMode();
            GameController.instance.FadeOut();
            // SceneManager.UnloadSceneAsync("BattleLevel");
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _levelEnd = false;
                Debug.Log(BattleController.battleNum);
                // For sprint 2
                if (BattleController.battleNum == 4)
                {
                    SceneManager.LoadScene("EndScene");
                    //SceneManager.LoadScene("MountainChallenge");
                }
                else if (BattleController.battleNum == 100)
                {
                    SceneManager.LoadScene("EndScene");
                }
                // Move to the next enemy
                else
                {
                    SceneManager.LoadScene("BattleMap");
                }
            }, 4f));
        }
        
        // Player loses
        if (BattleController.instance.player.Health_Current <= 0)
        {
            GameController.instance.DisableBattleMode();
            BattleController.battleNum = 100;
            SceneManager.LoadScene("EndScene");
        }
    }
}
