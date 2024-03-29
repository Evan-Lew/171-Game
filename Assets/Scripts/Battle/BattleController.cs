using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    [HideInInspector] bool enable_BattleController = false;

    // Main core script that will be called in different other objects
    public static BattleController instance;

    [SerializeField] private DeckSystem _script_DeckSystem;
    [SerializeField] private HandManager _script_HandManager;
    [HideInInspector] public bool startDrawingCards = true;
    public int startingCardsAmount;
    public float TurnChangeAnimationDuration = 1.5f;
    [HideInInspector]public enum TurnOrder { start, playerPhase, playerEndPhase, EnemyPhase, EnemyEndPhase }
    [HideInInspector]public enum TurnType { player, enemy }
    [HideInInspector]public TurnOrder currentPhase;
    [HideInInspector]public TurnOrder lastPhase = TurnOrder.playerPhase;
    [HideInInspector]public TurnOrder nextPhase;
    [HideInInspector] public bool enableUsingCard = false;
    [HideInInspector] public bool enableCardActivation = false;

    // For priority system
    [HideInInspector] public Character player, enemy;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] private EnemyAi _script_EnemyAI;
    [SerializeField] private BattleLog _script_BattleLog;
    [HideInInspector] public bool enableTurnUpdate = false;

    [SerializeField] Animator animatorPlayerTurn, animatorEnemyTurn, animatorFadeInOut;
    
    // Reference to BattleLog
    public BattleLog battleLog;
    // Reference to Karma Scale
    public KarmaScale karmaScale;

    [SerializeField] GameObject EndTurnText;
    [SerializeField] GameObject EnemyTurnText;

    // Used in BattleLevelSetup
    public static int battleNum = 0;
    public static int totalLevel = 0;
    public static double end_HP = 35;

    private void Awake()
    {
        instance = this;
    }
    
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     Debug.Log("Developer Tool: Change Enemy Health");
        //     enemy.Health_Current = 0;
        // }
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     Debug.Log("Developer Tool: Change Player Health");
        //     player.Health_Current = 0;
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     Debug.Log("Developer Tool: Draw a Card");
        //     _script_DeckSystem.DrawCardToHand();
        // }
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     Debug.Log("Developer Tool: Remove");
        //     _script_HandManager.Clear();
        // }

        // The battle controller will be enabled only if the battle is happened
        if (enable_BattleController)
        {
            if (enableTurnUpdate)
            {
                TurnUpdate();
            }
        }
    }
    
    // Set this overall script active
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

    // Setup this function
    public void SetUp()
    {
        player = GameObject.Find("Player").GetComponent<Character>();
        enemy = GameObject.Find("Enemy").GetComponent<Character>();
        if (startDrawingCards)
        {
            _script_DeckSystem.DrawMultipleCards(startingCardsAmount);
        }
        currentPhase = TurnOrder.start;
        nextPhase = TurnOrder.playerPhase;
        _script_PrioritySystem.AddCharacters(player);
        _script_PrioritySystem.AddCharacters(enemy);
        _script_BattleLog.Setup();
        enableTurnUpdate = true;
        SetActive(true);
    }

    // Reset the battle controller
    public void Clear()
    {
        enableTurnUpdate = false;
        _script_BattleLog.Clear();
        EffectDictionary.instance.ParticlesReset();
        EffectDictionary.instance.ManipulatorFullReset();
        SetActive(false);
        // Reset priority
        _script_PrioritySystem.ResetPriority(player);
        _script_PrioritySystem.ResetPriority(enemy);
        // Reset Scale/Orbs
        karmaScale.resetScale();
        karmaScale.resetOrbs();
    }
    
    void TurnUpdate()
    {
        enableTurnUpdate = false;

        // Special handling AtPlayerEndPhase
        if (currentPhase == TurnOrder.playerEndPhase)
        {
            SpecialHandling_AtEndPlayerTurn();
        }
        else if (currentPhase == TurnOrder.EnemyEndPhase)
        {
            currentPhase = nextPhase;
            BattleController.instance.enableTurnUpdate = true;
        }
        else if (currentPhase == TurnOrder.start)
        {
            TurnChangeAnimation(TurnType.player);
            lastPhase = TurnOrder.playerPhase;
            currentPhase = TurnOrder.playerPhase;
            EndTurnText.SetActive(true);
            EnemyTurnText.SetActive(false);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_DeckSystem.DrawCardToHand();
                enableUsingCard = true;
                enableCardActivation = true;
            }, TurnChangeAnimationDuration));
        }
        // Player turn and previous turn was the player
        else if (currentPhase == TurnOrder.playerPhase && lastPhase == TurnOrder.playerPhase)
        {
            if (_script_HandManager.player_hands_holdCards.Count == 0)
            {
                lastPhase = TurnOrder.playerPhase;
                currentPhase = TurnOrder.EnemyPhase;
                enableTurnUpdate = true;
            }
            else
            {
                enableUsingCard = true;
                enableCardActivation = true;
            }
        }
        // Player turn and previous turn was the enemy
        else if (currentPhase == TurnOrder.playerPhase && lastPhase == TurnOrder.EnemyPhase)
        {
            // Disable mouse hover
            //GameController.instance.enableMouseEffectOnCard = false;
            
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                TurnChangeAnimation(TurnType.player);
                EndTurnText.SetActive(true);
                EnemyTurnText.SetActive(false);
            }, TurnChangeAnimationDuration + 0.5f));
            
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_DeckSystem.DrawCardToHand();
                enableUsingCard = true;
                enableCardActivation = true;
                //GameController.instance.enableMouseEffectOnCard = true;
            }, TurnChangeAnimationDuration + 1.5f));
        }
        // Enemy turn and previous turn was the enemy
        else if (currentPhase == TurnOrder.EnemyPhase && lastPhase == TurnOrder.EnemyPhase)
        {
            enableUsingCard = false;
            enableCardActivation = false;
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_EnemyAI.isActioned = false;
                _script_EnemyAI.EnemyAction(enemy.CharacterName);
                battleLog.EnemyAttackPopup();
            }, 2.5f));
        }
        // Enemy turn and previous turn was the player
        else if (currentPhase == TurnOrder.EnemyPhase && lastPhase == TurnOrder.playerPhase)
        {
            enableUsingCard = false;
            enableCardActivation = false;
            TurnChangeAnimation(TurnType.enemy);
            EndTurnText.SetActive(false);
            EnemyTurnText.SetActive(true);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_EnemyAI.isActioned = false;
                _script_EnemyAI.EnemyAction(enemy.CharacterName);
                battleLog.EnemyAttackPopup();
            }, TurnChangeAnimationDuration + 0.5f));
        }
    }
    
    // Used for special card attacks
    void SpecialHandling_AtEndPlayerTurn()
    {
        currentPhase = nextPhase;
        BattleController.instance.enableTurnUpdate = true;
        // if (enemy.CharacterName == "Ink Golem")
        // {
        //     // _script_EnemyAI.CastUniqueAbility_Golem();
        //     currentPhase = nextPhase;
        // }
        // else
        // {
        //     currentPhase = nextPhase;
        //     BattleController.instance.enableTurnUpdate = true;
        // }
    }

    void TurnChangeAnimation(TurnType type)
    {
        SoundManager.PlaySound("sfx_Transition", 0.05f);
        if (type == TurnType.player)
        {
            animatorFadeInOut.SetTrigger("Play");
            animatorPlayerTurn.SetTrigger("Play");
        }
        else if (type == TurnType.enemy)
        {
            animatorFadeInOut.SetTrigger("Play");
            animatorEnemyTurn.SetTrigger("Play");
        }
    }

    public void playerEndTurn()
    {
        currentPhase = TurnOrder.EnemyPhase;
        lastPhase = TurnOrder.playerPhase;
        BattleController.instance.enableTurnUpdate = true;
    }

    public void ResetArmorSymbol()
    {
        enemy.ResetArmorSymbol();
        player.ResetArmorSymbol();
    }
}
