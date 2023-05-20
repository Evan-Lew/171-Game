using UnityEngine;

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
    [HideInInspector]public enum TurnType { player, enemy}
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
            Debug.Log("drawing");
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
        SetActive(false);
        // Reset priority
        _script_PrioritySystem.ResetPriority(player);
        _script_PrioritySystem.ResetPriority(enemy);
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
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                TurnChangeAnimation(TurnType.player);
            }, TurnChangeAnimationDuration + 0.5f));
            
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_DeckSystem.DrawCardToHand();
                enableUsingCard = true;
                enableCardActivation = true;
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
        SoundManager.PlaySound("sfx_Transition", 0.1f);
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
}
