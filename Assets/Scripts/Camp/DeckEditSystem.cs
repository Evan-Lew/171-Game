using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckEditSystem : MonoBehaviour
{

    [SerializeField] DeckSystem _script_DeckSystem;
    [SerializeField] Transform Pos_DisplayCardCandidates;
    [SerializeField] List<Card_Basedata> CardCandidatesList = new();


    Card InstantiateCard(Card cardPrefab, Card_Basedata cardData, Transform initTransform)
    {

        Card newCard = Instantiate(cardPrefab, initTransform.position, initTransform.rotation);
        newCard.cardData = cardData;
        newCard.cardState = Card.state.DeckCandidate;
        return newCard;
    }

    void AddCardToDeck(Card_Basedata card)
    {
        _script_DeckSystem.deckToUse.Add(card);
    }

    void RemoveCardFromDeck(Card_Basedata card)
    {
        _script_DeckSystem.deckToUse.RemoveAt(_script_DeckSystem.deckToUse.FindIndex(current => current.ID == card.ID));
    }

}
