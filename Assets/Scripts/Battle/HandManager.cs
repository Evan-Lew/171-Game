using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    bool enable_HandManager = false;

    // List for cards on the hand
    public List<Card> player_hands_holdCards = new List<Card>();
    // List for all card position can be hold in hands
    public List<Vector3> player_hands_holdsCardsPositions = new List<Vector3>();

    public Transform minPos, maxPos;

    Character player;
    Character enemy;
    [SerializeField] PrioritySystem _script_PrioritySystem;
    // For hovering animation
    public Vector3 hoveringAdjustment = new Vector3(0.5f, 0, 0);
    public List<Vector3> player_hands_holdsCardsPositionsHovering = new List<Vector3>();

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
        foreach (Card card in player_hands_holdCards)
        {
            Destroy(card.gameObject);
        }
        player_hands_holdCards.Clear();
        player_hands_holdsCardsPositions.Clear();
        SetActive(false);
    }

    // Set card positions in hands
    public void SetCardPositionsInHand()
    {
        // Reset
        player_hands_holdsCardsPositions.Clear();
        Vector3 distanceBetweenPoints = Vector3.zero;

        //Debug.Log(player_hands_holdCards.Count);

        // Calculate how cards will be placed by the number of cards on the hand
        // More than 2 cards
        if (player_hands_holdCards.Count == 1)
        {
            player_hands_holdCards[0].isInHand = true;
            player_hands_holdCards[0].handPosition = 0;
            player_hands_holdCards[0].cardState = Card.state.Handcard;
            player_hands_holdsCardsPositions.Add(minPos.position + (maxPos.position - minPos.position) / 2);
            player_hands_holdCards[0].MoveToPoint(player_hands_holdsCardsPositions[0], minPos.rotation);
        }
        else if (player_hands_holdCards.Count == 2)
        {
            for (int i = 0; i < player_hands_holdCards.Count; i++)
            {
                player_hands_holdCards[i].isInHand = true;
                player_hands_holdCards[i].handPosition = i;
                player_hands_holdCards[i].cardState = Card.state.Handcard;
                // Find the distance between each card
                player_hands_holdsCardsPositions.Add(minPos.position + (i + 1) * (maxPos.position - minPos.position) / 3);
                // Move card &&  adjust the angle to make it layerly sorted
                player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositions[i], minPos.rotation);
            }
        }
        else if (player_hands_holdCards.Count > 2)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (player_hands_holdCards.Count - 1);
            for (int i = 0; i < player_hands_holdCards.Count; i++)
            {
                player_hands_holdCards[i].isInHand = true;
                player_hands_holdCards[i].handPosition = i;
                player_hands_holdCards[i].cardState = Card.state.Handcard;

                // Find the distance between each card
                player_hands_holdsCardsPositions.Add(minPos.position + (distanceBetweenPoints * i));

                // Move card && adjust the angle to make it layered sorted
                player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositions[i], minPos.rotation);
            }
        }
    }

    // Hovering animation
    public void MoveOtherCardAtHovering(Card cardIsHovering)
    {
        player_hands_holdsCardsPositionsHovering.AddRange(player_hands_holdsCardsPositions);
        for (int i = 0; i < player_hands_holdsCardsPositionsHovering.Count; i++)
        {
            // Move to left
            if (i < cardIsHovering.handPosition)
            {
                player_hands_holdsCardsPositionsHovering[i] += hoveringAdjustment;
                // Move card &&  adjust the angle to make it layered sorted
                player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositionsHovering[i], minPos.rotation);
            }
            else if (i > cardIsHovering.handPosition)
            {
                player_hands_holdsCardsPositionsHovering[i] -= hoveringAdjustment;
                // Move card && adjust the angle to make it layered sorted
                player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositionsHovering[i], minPos.rotation);
            }
        }
    }

    public void MoveOtherCardAtHovering_Reset()
    {
        player_hands_holdsCardsPositionsHovering.Clear();
        SetCardPositionsInHand();
    }

    // Remove cards from hand
    public void RemoveCardFromHand(Card cardToRemove)
    {
        // Validate to check if the card is the one we want to remove
        if (player_hands_holdCards[cardToRemove.handPosition] == cardToRemove)
        {
            SoundManager.PlaySound("sfx_Card_Place", 0.25f);
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
        SoundManager.PlaySound("sfx_Card_Draw", 0.1f);
        player_hands_holdCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    //==============================================
    //         Helper Function for this script
    //==============================================

}
