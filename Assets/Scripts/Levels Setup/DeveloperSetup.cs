using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeveloperSetup : MonoBehaviour
{
    DeckSystem _deckSystem;
    HandManager _handManager;

    [Header("List of Cards for the Deck")]
    [SerializeField] List<Card_Basedata> developerCards;

    [Header("Character Names")]
    [SerializeField] String playerName;
    [SerializeField] String enemyName;
    
    private void Start()
    {
        _deckSystem = GameObject.Find("Deck System").GetComponent<DeckSystem>();
        _handManager = GameObject.Find("Hand System").GetComponent<HandManager>();
        StartTheBattle();
    }

    void StartTheBattle()
    {
        _deckSystem.deckToUse.Clear();

        // Add the cards specified in the list to the deck to be used
        foreach (Card_Basedata card in developerCards)
        {
            _deckSystem.deckToUse.Add(card);
        }
        
        GameController.instance.DeveloperBattleSetup(playerName, enemyName);
    }
}
