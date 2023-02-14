using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleController : MonoBehaviour
{
    [HideInInspector] bool enable_BattleController = false;

    //main core script that will be called in different other objects
    public static BattleController instance;

    [SerializeField] private DeckSystem _script_DeckSystem;
    public bool startDrawingCrads = true;
    public int startingCardsAmount;
    public enum TurnOrder { playerPhase, EnemyPhase }
    public TurnOrder currentPhase;

    //for priority system
    private Character player, enemy;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] private EnemyAi _script_EnemyAi;
    public TMP_Text _Text_Turn, _Text_PlayerPriority, _Text_EnemyPriority;

    bool turnChangedToPlayer = false;
    //public Card_Basedata currentUsingCard; 

    [SerializeField] Animator animator_fadeInOut, animator_PlayerTurn, animator_Enemy;


    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //the battle controll will be enabled only if the battle is happened
        if (enable_BattleController)
        {
            updateText();
            TurnUpdate();
        }

        //for testing, comment it out if you see this during the testing
        if (Input.GetKeyDown(KeyCode.K))
        {
            animator_fadeInOut.SetTrigger("Play");
            animator_PlayerTurn.SetTrigger("Play");
        }


        //if (animator_fadeInOut.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        //{
        //    Debug.Log("Animation finished!");
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            animator_fadeInOut.SetTrigger("Play");
            animator_Enemy.SetTrigger("Play");
        }
        //test done

    }


    //set this overall script active
    public void SetActive(bool setFlag)
    {
        if (setFlag)
        {
            enable_BattleController = true;
        }
        else
        {
            enable_BattleController = false;
        }
    }

    //setup this function
    public void SetUp()
    {
        player = GameObject.Find("Player").GetComponent<Character>();
        enemy = GameObject.Find("Enemy").GetComponent<Character>();
        if (startDrawingCrads)
        {
            _script_DeckSystem.DrawMultipleCards(startingCardsAmount);
        }
        currentPhase = TurnOrder.playerPhase;
        _script_PrioritySystem.AddCharacters(player);
        _script_PrioritySystem.AddCharacters(enemy);
        SetActive(true);
    }


    public void ProcessPriorityTurnControl()
    {
        Character result;
        result = _script_PrioritySystem.getNextTurnCharacter();
        if (result == player)
        {
            currentPhase = TurnOrder.playerPhase;
        }
        else
        {
            currentPhase = TurnOrder.EnemyPhase;
        }
    }


    void TurnUpdate()
    {

        //check if current phase is the last one
        if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;

        }

        //trigger action function
        switch (currentPhase)
        {
            case TurnOrder.playerPhase:
                //Debug.Log("Right now is playerPhase");

                if (turnChangedToPlayer)
                {
                    _script_DeckSystem.DrawCardToHand();
                }
                turnChangedToPlayer = false;


                _Text_Turn.text = currentPhase.ToString();
                _Text_Turn.color = Color.white;
                break;

            case TurnOrder.EnemyPhase:
                turnChangedToPlayer = true;
                _script_EnemyAi.EnemyUseAction();
                //Debug.Log("Right now is EnemyPhase");
                _Text_Turn.text = currentPhase.ToString();
                _Text_Turn.color = Color.red;
                break;
        }

    }

    void updateText()
    {
        _Text_PlayerPriority.text = _script_PrioritySystem.priorityDict[player].ToString();
        _Text_EnemyPriority.text = _script_PrioritySystem.priorityDict[enemy].ToString();
    }



    //====================================
    //         Debug Only
    //====================================


    //void playerUseCard()
    //{
    //    if (Input.GetKeyDown(KeyCode.Keypad7))
    //    {
    //        _script_PrioritySystem.AddCost(player, 2);
    //        ProcessPriorityTurnControl();
    //        _Text_PlayerPriority.text = _script_PrioritySystem.priorityDict[player].ToString();
    //    }
    //}

    //void enemyUseCard()
    //{
    //    if (Input.GetKeyDown(KeyCode.Keypad8))
    //    {
    //        _script_PrioritySystem.AddCost(enemy, 1);
    //        ProcessPriorityTurnControl();
    //        _Text_EnemyPriority.text = _script_PrioritySystem.priorityDict[enemy].ToString();
    //    }
    //}


    //public void changeTurnWithKey(KeyCode key)
    //{
    //    if (Input.GetKeyDown(key))
    //    {
    //        currentPhase++;

    //    }
    //}
    //====================================
    //         Debug End
    //====================================

}
