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
    List<Card> Cards_FromCandidates = new();
    List<Vector3> CardCandidatesPositionList = new();
    [SerializeField] float candidateDistance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCandidates();
        }
    }


    //this function is used to reset entire Candidate system
    public void ResetCandidate()
    {
        CardCandidatesList.Clear();
        CardCandidatesPositionList.Clear();
        Cards_FromCandidates.Clear();
    }

    //this function  will be called when ever you have a group of card_database wants to generate
    public void SpawnCandidates()
    {
        Debug.Log("Hello");
        Cards_FromCandidates.Clear();
        for (int i = 0; i < CardCandidatesList.Count; i++)
        {
            InstantiateCandidateCard(CardPrefab, CardCandidatesList[i], Pos_DisplayCardCandidates);
        }
        SetCandidateCardPos();
    }

    void SetCandidateCardPos()
    {
        Vector3 distanceBetween;
        CardCandidatesPositionList.Clear();
        if (CardCandidatesList.Count - 1 == 0)
        {
            distanceBetween = (Pos_CandidatesMax.position - Pos_CandidatesMin.position) / 2;
            for (int i = 0; i < CardCandidatesList.Count; i++)
            {
                Cards_FromCandidates[i].CandidatePosition = i;
                Cards_FromCandidates[i].cardState = Card.state.DeckCandidate;
                CardCandidatesPositionList.Add(Pos_CandidatesMin.position + (distanceBetween));
                Cards_FromCandidates[i].MoveToPoint(CardCandidatesPositionList[i], Cards_FromCandidates[i].transform.rotation);
            }
        }
        else
        {
            distanceBetween = (Pos_CandidatesMax.position - Pos_CandidatesMin.position) / (CardCandidatesList.Count - 1);
            for (int i = 0; i < CardCandidatesList.Count; i++)
            {
                Cards_FromCandidates[i].CandidatePosition = i;
                Cards_FromCandidates[i].cardState = Card.state.DeckCandidate;
                CardCandidatesPositionList.Add(Pos_CandidatesMin.position + (distanceBetween * i));
                Cards_FromCandidates[i].MoveToPoint(CardCandidatesPositionList[i], Cards_FromCandidates[i].transform.rotation);
            }
        }
    }


    void InstantiateCandidateCard(Card cardPrefab, Card_Basedata cardData, Transform SpawnTransform)
    {
        Card newCard = Instantiate(cardPrefab, SpawnTransform.position, SpawnTransform.rotation);
        newCard.cardData = cardData;
        newCard.cardState = Card.state.DeckCandidate;
        newCard.loadCard();
        Cards_FromCandidates.Add(newCard);
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
