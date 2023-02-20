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
    public float TurnChangeAnimationDuration = 2f;
    [HideInInspector]public enum TurnOrder { start, playerPhase, playerEndPhase, EnemyPhase, EnemyEndPhase }
    [HideInInspector]public enum TurnType { player, enemy}
    [HideInInspector]public TurnOrder currentPhase;
    [HideInInspector]public TurnOrder nextPhase;
    [HideInInspector] public bool enableUsingCard = false;
    [HideInInspector] public bool enableCardActivation = false;

    //for priority system
    private Character player, enemy;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] private EnemyAi _script_EnemyAi;
    [HideInInspector] public bool enableTurnUpdate = false;
    [HideInInspector] public bool enableEndTurn = false;

    [SerializeField] Animator animator_fadeInOut, animator_PlayerTurn, animator_EnemyTurn;


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

       // Debug.Log(currentPhase + " Next " + nextPhase);

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
        currentPhase = TurnOrder.start;
        nextPhase = TurnOrder.playerPhase;
        _script_PrioritySystem.AddCharacters(player);
        _script_PrioritySystem.AddCharacters(enemy);
        enableTurnUpdate = true;
        SetActive(true);
    }


    void TurnUpdate()
    {
        enableTurnUpdate = false;


        if (currentPhase == TurnOrder.start && nextPhase == TurnOrder.playerPhase)
        {
            currentPhase = nextPhase;
            TurnChangeAnimation(TurnType.player);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_DeckSystem.DrawCardToHand();
                enableUsingCard = true;
                enableCardActivation = true;


            }, TurnChangeAnimationDuration));
        }
        else if (currentPhase == TurnOrder.playerPhase && nextPhase == TurnOrder.playerPhase)
        {

            _script_DeckSystem.DrawCardToHand();
            enableUsingCard = true;
            enableCardActivation = true;
        }
        else if (currentPhase == TurnOrder.playerPhase && nextPhase == TurnOrder.EnemyPhase)
        {
            currentPhase = nextPhase;
            enableUsingCard = false;
            enableCardActivation = false;
            TurnChangeAnimation(TurnType.enemy);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_EnemyAi.isActioned = false;
                _script_EnemyAi.EnemyAction(enemy.CharacterName);

            }, TurnChangeAnimationDuration));
        }
        else if(currentPhase == TurnOrder.EnemyPhase && nextPhase == TurnOrder.EnemyPhase)
        {
            enableUsingCard = false;
            enableCardActivation = false;
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_EnemyAi.isActioned = false;
                _script_EnemyAi.EnemyAction(enemy.CharacterName);

            }, 1.5f));
        }else if(currentPhase == TurnOrder.EnemyPhase && nextPhase == TurnOrder.playerPhase)
        {
            TurnChangeAnimation(TurnType.player);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_DeckSystem.DrawCardToHand();
                enableUsingCard = true;
                enableCardActivation = true;


            }, TurnChangeAnimationDuration));
        }
        
    }



    void SpecialHandling_AtEndPlayerTurn()
    {


        //if (enemy.CharacterName == "Golem")
        //{


        //    _script_EnemyAi.CastUniqueAbility_Golem();



        //}
        //else
        //{
        //    //animator_fadeInOut.SetTrigger("Play");
        //    //animator_Enemy.SetTrigger("Play");

        //}


    }

    void TurnChangeAnimation(TurnType type)
    {
        if (type == TurnType.player)
        {
            animator_fadeInOut.SetTrigger("Play");
            animator_PlayerTurn.SetTrigger("Play");
        }
        else if (type == TurnType.enemy)
        {
            animator_fadeInOut.SetTrigger("Play");
            animator_EnemyTurn.SetTrigger("Play");
        }
    }
}
