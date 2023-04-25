using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleLevelSetup : MonoBehaviour
{
    DeckSystem _deckSystem;
    HandManager _handManager;

    [Header("List of Cards to start the deck with")]
    [SerializeField] List<Card_Basedata> startingCards;

    [SerializeField] Character_Basedata[] listOfEnemies;

    bool _levelEnd = false;
    
    private void Start()
    {
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
        
        // GameController.instance.DeveloperBattleSetup(playerName, enemyName);
        Character_Basedata enemy = listOfEnemies.Where(obj => obj.characterName == listOfEnemies[BattleController.battleNum].characterName).SingleOrDefault();
        // Character_Basedata enemy = listOfEnemies.Where(obj => obj.characterName == listOfEnemies[1].characterName).SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.battleCondition = true;
    }

    void LevelManagement()
    {
        // Player wins
        if (BattleController.instance.enemy.Health_Current <= 0 && _levelEnd == false)
        {
            BattleController.battleNum++;
            _levelEnd = true;
        }
        
        // Player loses
        if (BattleController.instance.player.Health_Current <= 0 || BattleController.battleNum >= 3)
        {
            GameController.instance.DisableBattleMode();
            SceneManager.LoadScene("EndScene");
        }
        
        if (_levelEnd)
        {
            GameController.instance.DisableBattleMode();
            
            // SceneManager.UnloadSceneAsync("BattleLevel");
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _levelEnd = false;
                Debug.Log(BattleController.battleNum);
                if (BattleController.battleNum == 2)
                {
                    SceneManager.LoadScene("StoryContinue");
                }
                else
                {
                    SceneManager.LoadScene("PickDeckLevel_1");     
                }
            }, 1f));
        }
    }
}
