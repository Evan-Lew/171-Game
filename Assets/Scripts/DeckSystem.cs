using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class DeckSystem : MonoBehaviour
{
    public static DeckSystem instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Card_Basedata> deckToUse = new List<Card_Basedata>();
    private List<Card_Basedata> activeCards = new List<Card_Basedata>();

    public Card cardToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        SetupDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupDeck()
    {
        activeCards.Clear();
        List<Card_Basedata> tempDeck = new List<Card_Basedata>();
        tempDeck.AddRange(deckToUse);

        while (tempDeck.Count > 0)
        {
            int selected = UnityEngine.Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
        }
    }

    public void DrawCardToHand()
    {
        if (activeCards.Count == 0)
        {
            
        }
    }
}
