using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DictionaryTest : MonoBehaviour
{
    public Card Card;

    delegate void funcHolder(Card card);
    funcHolder funcHolder_Cards;

    Dictionary<int, funcHolder> effectDictionary = new Dictionary<int, funcHolder>();

    void printCardName(Card card)
    {
        //Debug.Log(card.cardName);
    }

    void printCardID(Card card)
    {
        Debug.Log(card.cardID);
    }

    void effect_1003(Character player, Character enemy, Card card)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        effectDictionary.Add(1001, printCardName);
        effectDictionary.Add(1002, printCardID);

        effectDictionary[1001](Card);
        
        //if (Card.cardID == 1001)
        //{
        //    funcHolder_Cards = printCardName;
        //    funcHolder_Cards(Card);
        //}else if(Card.cardID == 1002)
        //{
        //    funcHolder_Cards = printCardID;
        //    funcHolder_Cards(Card);
        //}
    }

}
