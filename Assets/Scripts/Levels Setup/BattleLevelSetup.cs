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

    bool _levelEnd = false;
    
    [SerializeField] String backgroundName;
    
    private void Start()
    {
        GameController.instance.battleCondition = true;
        // Change background
        GameController.instance.ChangeBackground(backgroundName);
        
        //GameController.instance.EndDialogue();
        
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
        _deckSystem.deckToUse.Clear();

        // Add the cards specified in the list to the deck to be used
        foreach (Card_Basedata card in startingCards)
        {
            _deckSystem.deckToUse.Add(card);
        }
        
        GameController.instance.battleCondition = true;
        
        // listOfEnemies
        // 0, 1, 2 == fodder enemies
        // 3 == elite enemy
        // 99 == boss

        if (BattleController.battleNum == 99)
        {
            enemy = listOfEnemies.Where(obj => obj.characterName == "Stone Rui Shi").SingleOrDefault();
        } else {
            enemy = listOfEnemies.Where(obj => obj.characterName == listOfEnemies[BattleController.battleNum].characterName).SingleOrDefault();
        }
        
        GameController.instance.StartTheBattle(enemy, true);
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
        
        // Player loses
        //  || BattleController.battleNum >= 3
        if (BattleController.instance.player.Health_Current <= 0)
        {
            GameController.instance.DisableBattleMode();
            BattleController.battleNum = 100;
            SceneManager.LoadScene("EndScene");
        }
        
        if (_levelEnd)
        {
            BattleController.end_HP = BattleController.instance.player.Health_Current;
            GameController.instance.DisableBattleMode();
            
            // SceneManager.UnloadSceneAsync("BattleLevel");
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _levelEnd = false;
                Debug.Log(BattleController.battleNum);
                if (BattleController.battleNum == 3)
                {
                    SceneManager.LoadScene("MountainChallenge");
                }
                else if (BattleController.battleNum == 100)
                {
                    SceneManager.LoadScene("EndScene");
                }
                else
                {
                    SceneManager.LoadScene("PickDeckLevel_1");     
                }

            }, 1f));
        }
    }
}
