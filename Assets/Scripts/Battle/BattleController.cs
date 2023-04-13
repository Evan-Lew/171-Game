using UnityEngine;

public class BattleController : MonoBehaviour
{
    [HideInInspector] bool enable_BattleController = false;

    // Main core script that will be called in different other objects
    public static BattleController instance;

    [SerializeField] private DeckSystem _script_DeckSystem;
    [SerializeField] private HandManager _script_HandManager;
    [HideInInspector]public bool startDrawingCards = true;
    public int startingCardsAmount;
    public float TurnChangeAnimationDuration = 2f;
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
    [SerializeField] private EnemyAI _script_EnemyAI;
    [SerializeField] private BattleLog _script_BattleLog;
    [HideInInspector] public bool enableTurnUpdate = false;

    [SerializeField] Animator animator_fadeInOut, animator_PlayerTurn, animator_EnemyTurn;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
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

    public void Clear()
    {
        enableTurnUpdate = false;
        _script_BattleLog.Clear();
        EffectDictionary.instance.ParticlesReset();
        SetActive(false);
    }
    
    void TurnUpdate()
    {
        enableTurnUpdate = false;

        //special handling AtPlayerEndPhase
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
        else if (currentPhase == TurnOrder.playerPhase && lastPhase == TurnOrder.playerPhase)
        {
            if(_script_HandManager.player_hands_holdCards.Count == 0)
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
        else if (currentPhase == TurnOrder.playerPhase && lastPhase == TurnOrder.EnemyPhase)
        {
            TurnChangeAnimation(TurnType.player);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_DeckSystem.DrawCardToHand();
                enableUsingCard = true;
                enableCardActivation = true;
            }, TurnChangeAnimationDuration));
        }
        else if (currentPhase == TurnOrder.EnemyPhase && lastPhase == TurnOrder.EnemyPhase)
        {
            enableUsingCard = false;
            enableCardActivation = false;
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_EnemyAI.isActioned = false;
                _script_EnemyAI.EnemyAction(enemy.CharacterName);

            }, 1.5f));
        }
        else if (currentPhase == TurnOrder.EnemyPhase && lastPhase == TurnOrder.playerPhase)
        {
            enableUsingCard = false;
            enableCardActivation = false;
            TurnChangeAnimation(TurnType.enemy);
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _script_EnemyAI.isActioned = false;
                _script_EnemyAI.EnemyAction(enemy.CharacterName);

            }, TurnChangeAnimationDuration));
        }
    }
    
    void SpecialHandling_AtEndPlayerTurn()
    {
        if (enemy.CharacterName == "Ink Golem")
        {
            _script_EnemyAI.CastUniqueAbility_Golem();
            currentPhase = nextPhase;
        }
        else
        {
            currentPhase = nextPhase;
            BattleController.instance.enableTurnUpdate = true;
        }
    }

    void TurnChangeAnimation(TurnType type)
    {
        SoundManager.PlaySound("sfx_Transition", 0.1f);
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
