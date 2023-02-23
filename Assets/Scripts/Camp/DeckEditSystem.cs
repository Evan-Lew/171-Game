using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckEditSystem : MonoBehaviour
{

    //add name.png to card.cs

    [SerializeField] DeckSystem _script_DeckSystem;
    [SerializeField] Transform Pos_DisplayCardCandidates, Pos_CandidatesMin, Pos_CandidatesMax, Pos_CardsDisplay;
    [SerializeField] Card CardPrefab;
    [SerializeField] List<Card_Basedata> CardCandidatesList = new();
    List<Card> Cards_ForPick = new();
    [SerializeField] int numberCandidate = 3;
    List<Vector3> CardForPickPositionList = new();
    [SerializeField] float candidateDistance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SpawnCandidates();
        }
    }


    //this function is used to reset entire Candidate system
    public void ResetCandidate()
    {
        CardCandidatesList.Clear();
        CardForPickPositionList.Clear();
        Cards_ForPick.Clear();
    }

    public void DestroyCurrentCardsForPick()
    {
        foreach(Card card in Cards_ForPick)
        {
            Destroy(card.gameObject);
        }
        Cards_ForPick.Clear();
    }

    //this function  will be called when ever you have a group of card_database wants to generate
    public void SpawnCardsForPick()
    {
        Debug.Log("Hello");
        DestroyCurrentCardsForPick();
        
        for (int i = 0; i < numberCandidate; i++)
        {
            InstantiateCandidateCard(CardPrefab, CardCandidatesList[Random.Range(0, CardCandidatesList.Count)], Pos_DisplayCardCandidates);
        }
        SetCandidateCardPos();
    }

    void SetCandidateCardPos()
    {
        Vector3 distanceBetween;
        CardForPickPositionList.Clear();
        if (Cards_ForPick.Count - 1 == 0)
        {
            distanceBetween = (Pos_CandidatesMax.position - Pos_CandidatesMin.position) / 2;
            for (int i = 0; i < Cards_ForPick.Count; i++)
            {
                Cards_ForPick[i].CandidatePosition = i;
                Cards_ForPick[i].cardState = Card.state.DeckCandidate;
                CardForPickPositionList.Add(Pos_CandidatesMin.position + (distanceBetween));
                Cards_ForPick[i].MoveToPoint(CardForPickPositionList[i], Cards_ForPick[i].transform.rotation);
            }
        }
        else
        {
            distanceBetween = (Pos_CandidatesMax.position - Pos_CandidatesMin.position) / (CardCandidatesList.Count - 1);
            for (int i = 0; i < Cards_ForPick.Count; i++)
            {
                Cards_ForPick[i].CandidatePosition = i;
                Cards_ForPick[i].cardState = Card.state.DeckCandidate;
                CardForPickPositionList.Add(Pos_CandidatesMin.position + (distanceBetween * i));
                Cards_ForPick[i].MoveToPoint(CardForPickPositionList[i], Cards_ForPick[i].transform.rotation);
            }
        }
    }


    void InstantiateCandidateCard(Card cardPrefab, Card_Basedata cardData, Transform SpawnTransform)
    {
        Card newCard = Instantiate(cardPrefab, SpawnTransform.position, SpawnTransform.rotation);
        newCard.cardData = cardData;
        newCard.cardState = Card.state.DeckCandidate;
        newCard.loadCard();
        Cards_ForPick.Add(newCard);
    }

    void AddCardToDeck(Card_Basedata card)
    {
        _script_DeckSystem.deckToUse.Add(card);
    }

    void RemoveCardFromDeck(Card_Basedata card)
    {
        _script_DeckSystem.deckToUse.RemoveAt(_script_DeckSystem.deckToUse.FindIndex(current => current.ID == card.ID));
    }



    void DisplayCurrentDeck()
    {

        //for (int i = 0; i < _script_DeckSystem.deckToUse.Count; i++)
        //{
        //    Card newCard = Instantiate(CardPrefab, Pos_CardsDisplay.position, Pos_CardsDisplay.rotation);
        //    newCard.cardData = _script_DeckSystem.deckToUse[i];
        //    newCard.cardState = Card.state.DeckDisplay;
        //    newCard.loadCard();
        //}
    }


    void UpdateDisplayCardPosition()
    {
        //for (int i = 0; i < player_hands_holdCards.Count; i++)
        //{
        //    _script_DeckSystem.deckToUse[i].hand = i;
        //    player_hands_holdCards[i].cardState = Card.state.Handcard;

        //    //find the distance between each card
        //    player_hands_holdsCardsPositions.Add(minPos.position + (distanceBetweenPoints * i));

        //    //move card &&  adjust the angle to make it layerly sorted
        //    player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositions[i], minPos.rotation);
        //}
    }

}
