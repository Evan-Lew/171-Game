using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Starter Deck", menuName = "Starter Deck", order = 3)]
public class StarterDeck_SO : ScriptableObject
{

    public string deckName;
    [TextArea]
    public string description;
    public Card_Basedata[] deck;
    public Sprite DeckSprite;
}
