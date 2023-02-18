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
    public enum TurnOrder { playerPhase, EnemyPhase }
    public TurnOrder currentPhase;
    private TurnOrder nextPhase;

    //for priority system
    private Character player, enemy;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] private EnemyAi _script_EnemyAi;
    [HideInInspector] public bool enableTurnUpdate = false;
    [HideInInspector] public bool enableEndTurn = false;

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

            if (enableTurnUpdate)
            {
                TurnUpdate();
            }

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
        if (result == player && currentPhase != TurnOrder.playerPhase)
        {
            //switch to player turn, trigger the animation
            nextPhase = TurnOrder.playerPhase;
            animator_fadeInOut.SetTrigger("Play");
            animator_PlayerTurn.SetTrigger("Play");

        }
        else if (result == enemy && currentPhase != TurnOrder.EnemyPhase)
        {
            //switch to enemy turn, trigger the animation
            nextPhase = TurnOrder.EnemyPhase;
            SpecialHandling_EndPlayerTurn();
        }

        enableEndTurn = false;
        enableTurnUpdate = true;
        currentPhase = nextPhase;

    }



    void TurnUpdate()
    {
        // Debug.Log(currentPhase + "" + nextPhase);
        if (currentPhase == TurnOrder.playerPhase)
        {
            if (!enableEndTurn)
            {
                Debug.Log("Process playerTurn");
                enableEndTurn = false;
                enableTurnUpdate = false;
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    //enmey turn end
                    _script_DeckSystem.DrawCardToHand();
                }, drawAfterAnimationSeconds));
            }
            else
            {
                Debug.Log("Process playerEndTurn");
                //player end
                ProcessPriorityTurnControl();
            }
        }
        else if (currentPhase == TurnOrder.EnemyPhase)
        {

            if (!enableEndTurn)
            {
                Debug.Log("Process EnemyTurn");
                enableEndTurn = false;
                enableTurnUpdate = false;
                _script_EnemyAi.isActioned = false;
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    _script_EnemyAi.EnemyAction(enemy.CharacterName);
                }, 3.5f));

            }
            else
            {
                Debug.Log("Process enemyEndTurn");
                //enmey turn end
                ProcessPriorityTurnControl();
            }
        }
    }




    void SpecialHandling_EndPlayerTurn()
    {


        if (enemy.CharacterName == "Golem")
        {


            _script_EnemyAi.CastUniqueAbility_Golem();
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                animator_fadeInOut.SetTrigger("Play");
                animator_Enemy.SetTrigger("Play");
            }, 1f));


        }
        else
        {
            animator_fadeInOut.SetTrigger("Play");
            animator_Enemy.SetTrigger("Play");

        }
    }
}
