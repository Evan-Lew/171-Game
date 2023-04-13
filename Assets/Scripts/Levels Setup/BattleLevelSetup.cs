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
    
    // private void Awake()
    // {
    //     GameController.instance.changePlayerSprite();
    // }
    
    private void Start()
    {
        GameController.instance.changePlayerSprite();
        _deckSystem = GameObject.Find("Deck System").GetComponent<DeckSystem>();
        _handManager = GameObject.Find("Hand System").GetComponent<HandManager>();
        StartTheBattle();
    }

    void StartTheBattle()
    {
        _deckSystem.deckToUse.Clear();

        // Add the cards specified in the list to the deck to be used
        foreach (Card_Basedata card in startingCards)
        {
            _deckSystem.deckToUse.Add(card);
        }
        
        //GameController.instance.DeveloperBattleSetup(playerName, enemyName);
        Character_Basedata enemy = listOfEnemies.Where(obj => obj.characterName == "Ink Golem").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.battleCondition = true;
    }
}
