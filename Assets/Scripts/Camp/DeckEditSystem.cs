using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class DeckEditSystem : MonoBehaviour
{
    public static DeckEditSystem instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] DeckSystem _script_DeckSystem;
    [SerializeField] Transform Pos_DisplayCardCandidates, Pos_CandidatesMin, Pos_CandidatesMax;
    [SerializeField] Card CardPrefab;
    [SerializeField] List<Card_Basedata> CardCandidatesList = new();
    public List<Card> Cards_ForPick = new();
    [SerializeField] int numberCandidate = 3;
    [SerializeField] List<Vector3> CardForPickPositionList = new();
    float candidateDistance;

    [HideInInspector]public bool isCardPicked = false;
    public TMP_Text DeckTotalText;
    
    public List<Card> testCardList = new();
    public Transform testPos;
    public Card_Basedata testCardData;

    [SerializeField] GameObject battleButton;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //InstantiateACard(testCardList, testCardData, testPos, Card.state.DeckDisplay);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //DestroyACardFromList(testCardList, "Payment");
        }

        if (_script_DeckSystem.deckToUse.Count == 10)
        {
            battleButton.SetActive(true);
        }
    }

    //===========================================================
    //                  DeckEditSystem API
    //===========================================================

    /*  Function that instantiate a card based on the given cardData, transform, and state
     *  Parameter:  Argument1:  CardList that used to hold new generated card
     *              Argument2:  CardData is which card you want to generate
     *              Argument3:  TargetTransform where you want the target been generated, include rotation. To change Scaling, Adjust the Y value on Spawning Point
     *              Argument4:  Card.state state, state determines the card behavior, state.DeckDisplay is static card, state.DeckCandidate allows you to pick
     *  Example Call:   InstantiateACard(testCardList, testCardData, testPos, Card.state.DeckDisplay);                                             
     */
    public void InstantiateACard(List<Card> CardList, Card_Basedata CardData, Transform TargetTransform, Card.state state)
    {
        Card newCard = Instantiate(CardPrefab, TargetTransform.position, TargetTransform.rotation);
        newCard.cardData = CardData;
        newCard.cardState = state;
        newCard.loadCard();
        CardList.Add(newCard);
    }

    /*  Function that destroy a list of cards
     *  Parameter:  Argument1:  CardList you want to destroy
     *  Example Call:   DestroyCardList(testCardList);
     */
    public void DestroyCardList(List<Card> CardList)
    {
        foreach (Card card in CardList)
        {
            Destroy(card.gameObject);
        }
        CardList.Clear();
    }
    
    /*  Function that destroy a card from the list
     *  Parameter:  Argument1:  CardList you want to process
     *              Argument2:  name of the game you want to destroy
     *  Example Call:   DestroyACardFromList(testCardList, "Payment");
     */
    public void DestroyACardFromList(List<Card> CardList, string targetCardName)
    {
        if (CardList.Count != 0)
        {
            int index = CardList.FindIndex(card => card.cardName == targetCardName);
            if (index == -1)
            {
                Debug.Log("Error: DestroyACardFromList() in DeckEditSystem. can't find the target Card in the list");
            }
            else
            {
                Destroy(CardList[index]);
                CardList.RemoveAt(index);
            }
        }
    }

    // Function that get the current Deck information
    // Return: List of card Basedata- Why not return list of cards, because cards are script for card Object
    // Also the card object has all components to make it work (model, texture), but card Basedata just a scriptable obj
    public List<Card_Basedata> GetCurrentDeck()
    {
        return _script_DeckSystem.deckToUse;
    }
    
    // Function that adding a Card_Basedata to deck. Note the deck will be modified until you call RemoveCardFromDeck
    public void AddCardToDeck(Card_Basedata card)
    {
        _script_DeckSystem.deckToUse.Add(card);
        isCardPicked = true;
    }

    // Function that removing a Card_Basedata from deck. Note the deck will be modified
    public void RemoveCardFromDeck(Card_Basedata card)
    {
        _script_DeckSystem.deckToUse.RemoveAt(_script_DeckSystem.deckToUse.FindIndex(current => current.ID == card.ID));
    }

    // Function that remove all cards from deck
    public void RemoveAllCardsFromDeck()
    {
        _script_DeckSystem.deckToUse.Clear();
    }


    // Function displaying all totally deck amount. Move DeckTotalText GameObject to adjust location
    public void UpdateText()
    {
        DeckTotalText.text = "Selected " + _script_DeckSystem.deckToUse.Count + "/" + "10 Cards";
    }


    // This is the function called by the button under UI Camp => button pick Card, once hit, this will be call one time
    // Using this as example as how SpawnCardsForPick() works
    public void SpawnCandidateForPick()
    {
        //Debug.Log("Spawning");
        SpawnCardsForPick(numberCandidate, CardCandidatesList, Pos_DisplayCardCandidates, Pos_CandidatesMin, Pos_CandidatesMax);
    }
    
    /*  Function that will spawn cards for picking/adding to deck in a row.
     *  Parameter:  Argument1: totalCandidateNum. Total number of cards will be spawned
     *              Argument2: CandidatesList. the list of optional cards where those cards will be generate from. To modify the rate
     *                          modify CandidatesList[Random.Range(0, CandidatesList.Count)] part in this function
     *              Argument3: DisplayFrom. Position where those cards start at
     *              Argument4: MinPos. Position determines the left edge of the row
     *              Argument5: MaxPos. Position determines the rightside of the row
     *              
     *  Example Call:   see above
     *  IMPORTANT:  the List, Cards_ForPick created as varible in this script will be used to store those cards.
     */
    void SpawnCardsForPick(int totalCandidateNum, List<Card_Basedata> CandidatesList, Transform DisplayFrom, Transform MinPos, Transform MaxPos)
    {
        if (_script_DeckSystem.deckToUse.Count != 10)
        {
            isCardPicked = false;
            DestroyCurrentCardsForPick();

            for (int i = 0; i < totalCandidateNum; i++)
            {
                InstantiateCandidateCard(CardPrefab, CandidatesList[Random.Range(0, CandidatesList.Count)], DisplayFrom);
            }
            SetCandidateCardPos(MinPos, MaxPos);
        }
    }
    
    //                DeckEditSystem API End
    //===========================================================
    
    // Use API Above and Ignore all function below
    //=================================================================================================================
    
    //===========================================================
    //                  Helper Functions
    //===========================================================
    
    //SpawnCardsForPick()
    /*  Function that reset cards position in range of Pos_CandidatesMin.pos and Pos_CandidatesMin.pos.
     *      Those two gameObject can be found under->Card System->Position-> Card Spawning Min, Max
     *  Return: None, once this is called, the card will move to target location by Lerp moving
     *  Important: Call this only once whenever the position needs to be changed such as new candidates will be spawned
     */
    void SetCandidateCardPos(Transform minPos, Transform maxPos)
    {
        Vector3 distanceBetween;
        CardForPickPositionList.Clear();
        if (Cards_ForPick.Count - 1 == 0)
        {
            distanceBetween = (maxPos.position - minPos.position) / 2;
            for (int i = 0; i < Cards_ForPick.Count; i++)
            {
                Cards_ForPick[i].CandidatePosition = i;
                Cards_ForPick[i].cardState = Card.state.DeckCandidate;
                CardForPickPositionList.Add(minPos.position + (distanceBetween));
                Cards_ForPick[i].MoveToPoint(CardForPickPositionList[i], Cards_ForPick[i].transform.rotation);
            }
        }
        else
        {
            distanceBetween = (maxPos.position - minPos.position) / (Cards_ForPick.Count - 1);
            for (int i = 0; i < Cards_ForPick.Count; i++)
            {
                Cards_ForPick[i].CandidatePosition = i;
                Cards_ForPick[i].cardState = Card.state.DeckCandidate;
                CardForPickPositionList.Add(minPos.position + (distanceBetween * i));
                Cards_ForPick[i].MoveToPoint(CardForPickPositionList[i], Cards_ForPick[i].transform.rotation);
            }
        }
    }

    //SpawnCardsForPick()
    void InstantiateCandidateCard(Card cardPrefab, Card_Basedata cardData, Transform SpawnTransform)
    {
        //GameObject instantiatedObject = Instantiate(prefabReference);
        //instantiatedObject.transform.localScale = new Vector3(newScale, newScale, newScale);
        
        Card newCard = Instantiate(cardPrefab, SpawnTransform.position, SpawnTransform.rotation);
        newCard.transform.localScale = new Vector3(150, 150, 150);
        newCard.cardData = cardData;
        newCard.cardState = Card.state.DeckCandidate;
        newCard.loadCard();
        Cards_ForPick.Add(newCard);
    }
    
    //SpawnCardsForPick()
    /*  this function is used to reset entire Candidate system
    */
    public void DestroyCurrentCardsForPick()
    {
        foreach (Card card in Cards_ForPick)
        {
            Destroy(card.gameObject);
        }
        Cards_ForPick.Clear();
    }
    
    public void RestartDeckSelecting()
    {
        DeckTotalText.text = "Selected 0/10 Cards";
        battleButton.SetActive(false);
    }
    //                  Helper Function End
    //===========================================================
}
