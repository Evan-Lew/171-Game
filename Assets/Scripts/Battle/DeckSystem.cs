using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeckSystem : MonoBehaviour
{

    public HandManager HandManager;

    // deckToUse is a public list for the cards we want to use
    // activeCards are the cards being played
    public List<Card_Basedata> deckToUse = new List<Card_Basedata>();
    //this is used to handle banish
    public List<Card_Basedata> deckForCurrentBattle = new List<Card_Basedata>();
    //cards still in the current deck waiting for draw
    public List<Card_Basedata> activeCards = new List<Card_Basedata>();

    // Reference to card that will be made in the world
    public Card cardToSpawn;

    public float timeBetweenDrawingCards = 5f;

    public int drawLimit = 7;
    public Transform drawFromPos;

    private void Awake()
    {
        initDeck();
    }

    void Start()
    {
        
        SetupDeck();
    }
    void Update()
    {
        //old version to draw card
         if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCardToHand();
        }
    }

    //copy prepared deck to the current battle deck
    void initDeck()
    {
        deckForCurrentBattle.Clear();
        deckForCurrentBattle.AddRange(deckToUse);
    }


    // Setup the activeCards deck. Note the deckForCurrentBattle will change if banish happened
    public void SetupDeck()
    {
        List<Card_Basedata> tempDeck = new List<Card_Basedata>();
        activeCards.Clear();
        tempDeck.Clear();
        //throw all cards into temp deck
        tempDeck.AddRange(deckForCurrentBattle);

        //tempDeck.AddRange(deckToUse);

        // Randomly add cards from deckToUse to activeCards
        while (tempDeck.Count > 0)
        {
            //random won't pick the right number
            int selectedIndex = UnityEngine.Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selectedIndex]);
            tempDeck.RemoveAt(selectedIndex);
        }
    }

  
    // Draw cards from activeCards
    public void DrawCardToHand()
    {
        //Debug.Log(activeCards.Count);
        //draw only if the hand cards not reaching the limit
        if (HandManager.player_hands_holdCards.Count <= drawLimit)
        {
            // Check if there are cards to draw from activeCards
            if (activeCards.Count == 0)
            {
                SetupDeck();
            }
            // Create a copy of the card prefab
            Card newCard = Instantiate(cardToSpawn, drawFromPos.position, transform.rotation);
            newCard.cardData = activeCards[0];
            newCard.loadCard();

            activeCards.RemoveAt(0);

            // Use HandManager static instance
            HandManager.AddCardToHand(newCard);
        }
    }


    ////// IMPORTANT: need to implement priority system into here
    ////public void DrawCardForPriority()
    ////{
    ////    DrawCardToHand();
    ////}

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultipleCoroutine(amountToDraw));
    }

    // Add delay between drawing cards at the start of the game
    IEnumerator DrawMultipleCoroutine(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();
            yield return new WaitForSeconds(timeBetweenDrawingCards);
        }
    }
}
