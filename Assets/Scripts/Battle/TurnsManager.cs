using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    public enum TurnOrder {playerPhase, EnemyPhase, ExtraPhase,  }
    public TurnOrder currentPhase;
    [HideInInspector]public bool isUsingExtraPhase;

    [SerializeField] private PrioritySystem _script_prioritySystem;

    public Character player, enemy;
    
    // Starting amount of cards
    public int startingCardsAmount = 6;

    // Start is called before the first frame update
    void Start()
    {
        TestPlay();
        DeckSystem.instance.DrawMultipleCards(startingCardsAmount);
    }

    // Update is called once per frame
    void Update()
    {
        Helper_TestPhaseWithKey(KeyCode.T);
    }


    public void TestPlay()
    {
        _script_prioritySystem.AddCharacters(player);
        _script_prioritySystem.AddCharacters(enemy);


       


    }

    public void AdvanceTurn()
    {
        //if extraPhase is not needed
        if (!isUsingExtraPhase)
        {
            //switch phases
            if(currentPhase == TurnOrder.playerPhase)
            {
  
                currentPhase = TurnOrder.EnemyPhase;
                Debug.Log("Right now is EnemyPhase");
            }
            else if(currentPhase == TurnOrder.EnemyPhase)
            {
                
                currentPhase = TurnOrder.playerPhase;
                Debug.Log("Right now is PlayerPhase");
            }

            //trigger action function
            switch (currentPhase)
            {
                case TurnOrder.playerPhase:

                    break;

                case TurnOrder.EnemyPhase:

                    break;

            }



        }
    }


    //==============================================
    //         Helper Function for this script
    //==============================================

    //used to test if the turn will be advanced
    void Helper_TestPhaseWithKey(KeyCode inputKey)
    {
        if (Input.GetKeyDown(inputKey))
        {
            AdvanceTurn();
        }
    }






}
