using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    bool enable_HandManager = false;


    //list for cards on the hand
    public List<Card> player_hands_holdCards = new List<Card>();
    //list for all card position can be hold in hands
    public List<Vector3> player_hands_holdsCardsPositions = new List<Vector3>();

    public Transform minPos, maxPos;

    Character player;
    Character enemy;
    [SerializeField] PrioritySystem _script_PrioritySystem;


    private void Update()
    {
        if (enable_HandManager)
        {
            //reserved for any check
        }
    }


    public void SetActive(bool setFlag)
    {
        if (setFlag)
        {
            enable_HandManager = true;
        }
        else
        {
            enable_HandManager = false;
        }
    }


    public void SetUp()
    {
        Clear();
        player = GameObject.Find("Player").GetComponent<Character>();
        enemy = GameObject.Find("Enemy").GetComponent<Character>();
        player_hands_holdsCardsPositions.Clear();
        SetCardPositionsInHand();
        SetActive(true);
    }

    public void Clear()
    {
        player_hands_holdCards.Clear();
        player_hands_holdsCardsPositions.Clear();
    }

    //set card positions in hands
    public void SetCardPositionsInHand()
    {
        //reset
        player_hands_holdsCardsPositions.Clear();
        Vector3 distanceBetweenPoints = Vector3.zero;

        //Debug.Log(player_hands_holdCards.Count);

        //calulate how cards will be placed by the number of cards on the hand
        if (player_hands_holdCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (player_hands_holdCards.Count - 1);
        }




        for (int i = 0; i < player_hands_holdCards.Count; i++)
        {
            player_hands_holdCards[i].isInHand = true;
            player_hands_holdCards[i].handPosition = i;
            player_hands_holdCards[i].cardState = Card.state.Handcard;

            //find the distance between each card
            player_hands_holdsCardsPositions.Add(minPos.position + (distanceBetweenPoints * i));

            //move card &&  adjust the angle to make it layerly sorted
            player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositions[i], minPos.rotation);
        }
    }


    //=================================> Optimization needed here later on
    // Remove cards from hand
    public void RemoveCardFromHand(Card cardToRemove)
    {
        // Validate to check if the card is the one we want to remove
        if (player_hands_holdCards[cardToRemove.handPosition] == cardToRemove)
        {
            SoundManager.PlaySound("sfx_Card_Place", 1);
            player_hands_holdCards.RemoveAt(cardToRemove.handPosition);
            //Calculation(enemy, player, cardToRemove);
        }
        else
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the correct card for removing!");
        }

        // Reset the card position
        SetCardPositionsInHand();
    }
    
    // Add a card to hand
    public void AddCardToHand(Card cardToAdd)
    {
        SoundManager.PlaySound("sfx_Card_Draw", 1);
        player_hands_holdCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }
    


        //==============================================
        //         Helper Function for this script
        //==============================================

}
