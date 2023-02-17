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
    public float drawAfterAnimationSeconds = 2f;
    public enum TurnOrder { playerPhase, EnemyPhase}
    public TurnOrder currentPhase;

    //for priority system
    private Character player, enemy;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] private EnemyAi _script_EnemyAi;
    public bool enableEndTurn = false;
    public float endPhaseWaitTime = 2f;

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
            TurnUpdate();
        }

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


    //testing for golem
    public void ProcessPriorityTurnControl()
    {
        Character result;
        result = _script_PrioritySystem.getNextTurnCharacter();
        if (result == player)
        {
            Debug.Log("switch to enemy");
            //switch to player turn, trigger the animation
            currentPhase = TurnOrder.playerPhase;
            animator_fadeInOut.SetTrigger("Play");
            animator_PlayerTurn.SetTrigger("Play");
        }
        else
        {
            //switch to enemy turn, trigger the animation
            Debug.Log("switch to enemy");
            currentPhase = TurnOrder.EnemyPhase;
            animator_fadeInOut.SetTrigger("Play");
            animator_Enemy.SetTrigger("Play");
            //just for testing golem
            EffectDictionary.instance.effectDictionary_Enemies[3]();
        }
    }


    void TurnUpdate()
    {
        
        if(currentPhase == TurnOrder.playerPhase)
        {
            if (!enableEndTurn)
            {
                if (turnChangedToPlayer)
                {
                    StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                    {
                        //enmey turn end
                        _script_DeckSystem.DrawCardToHand();
                    }, drawAfterAnimationSeconds));


                }
                turnChangedToPlayer = false;
            }
            else
            {
                //end turn pause
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    //player end
                    currentPhase = TurnOrder.EnemyPhase;
                    enableEndTurn = false;



                }, endPhaseWaitTime));
            }
        }else if(currentPhase == TurnOrder.EnemyPhase)
        {
            if (!enableEndTurn)
            {
                turnChangedToPlayer = true;
                _script_EnemyAi.EnemyUseAction();
            }
            else
            {
                //end turn pause
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    //enmey turn end
                    currentPhase = TurnOrder.playerPhase;
                    enableEndTurn = false;
                }, endPhaseWaitTime));
            }
        }
    }
}
