using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // HandManager static instance
 

    //list for cards on the hand
    public List<Card> player_hands_holdCards = new List<Card>();
    //list for all card position can be hold in hands
    public List<Vector3> player_hands_holdsCardsPositions = new List<Vector3>();

    public Transform minPos, maxPos;
    public Transform deckPos;

    [SerializeField] Character player;
    [SerializeField] Character enemy;
    [SerializeField] PrioritySystem _script_PrioritySystem;


    // Start is called before the first frame update
    void Start()
    {
   

        // Helper_Search_CardsInHands();
        SetCardPositionsInHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //set card positions in hands
    public void SetCardPositionsInHand()
    {
        // public void SetCardPositionsInHand(isDrawing)
        //abandoned preremeter


        Vector3 distanceBetweenPoints = Vector3.zero;

        //reset the hands
        player_hands_holdsCardsPositions.Clear();

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

            //if (isDrawing)
            //{
            //    //set all cards to deck's posisiton and draw it from there
            //    //player_hands_holdCards[i].transform.position = deckPos.position;
            //}

            //find the distance between each card
            player_hands_holdsCardsPositions.Add(minPos.position + (distanceBetweenPoints * i));

            //move card &&  adjust the angle to make it layerly sorted
            //player_hands_holdCards[i].transform.position = player_hands_holdsCardsPositions[i];
            //player_hands_holdCards[i].transform.rotation = minPos.rotation;
            player_hands_holdCards[i].MoveToPoint(player_hands_holdsCardsPositions[i], minPos.rotation);
        }
    }


    //=================================> Optimization needed here later on
    //remove cards from hand
    public void RemoveCardFromHand(Card cardToRemove)
    {
        //validation check to see if the card is the one we want to remove
        if (player_hands_holdCards[cardToRemove.handPosition] == cardToRemove)
        {
            player_hands_holdCards.RemoveAt(cardToRemove.handPosition);
            //Calculation(enemy, player, cardToRemove);

            
        }
        else
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the correct card for removing!");
        }

        //reset the card position
        SetCardPositionsInHand();
    }
    
    //add card to hand
    public void AddCardToHand(Card cardToAdd)
    {
        player_hands_holdCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }
    

    //void Calculation(Character target, Character caster, Card usedCard)
    //{
    //    caster.Priority_Current += usedCard.priorityCost;
    //    target.Health_Current -= usedCard.damageDealt;



    //    Debug.Log("player Priority: " + player.Priority_Current);
    //    Debug.Log("Enemy Priority: " + enemy.Priority_Current);
    //    Character nextCharacter = _script_PrioritySystem.getNextTurnCharacter();

    //}




        //==============================================
        //         Helper Function for this script
        //==============================================

        void Helper_Search_CardsInHands()
    {
        //GameObject targetObject = this.gameObject.transform.Find("Player Cards").gameObject;
        //int count = targetObject.transform.childCount;
        //Debug.Log(count);
        //for(int i = 0; i < count; i++)
        //{
        //    player_hands_holdCards.Add(targetObject.transform.GetChild(i).gameObject);
        //}
    }


}
