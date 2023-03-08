using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;
using static CoroutineUtil;
using System.Linq;
using TMPro;

//手卡
//Player_HandCard = _script_HandSystem.player_hands_holdCards.Count
//牌库
//卡面编辑后的总卡组 Player_DeckTotal = _script_DeckSystem.deckToUse.Count();
//当前剩余的卡      Player_DeckActivate = _script_DeckSystem.activeCards.Count();
//不算上消失的卡组   Player_DeckCurrent = _script_DeckSystem.deckForCurrentBattle.Count()


public class EffectDictionary : MonoBehaviour
{
    // Move any extra calculations to the card effect function for optimization.
    // Make tagged function SIMPLY.
    // Easy called from all other script without referencing
    // Example:
    public static EffectDictionary instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private DeckSystem _script_DeckSystem;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] private HandManager _script_HandSystem;
    Character player, enemy;
    GameObject playerObj, enemyObj;

    [Header("List of banished cards")]
    public List<Card_Basedata> BanishPool;
    [Header("List of return cards")]
    public List<Card_Basedata> ReturnPool;

    [HideInInspector] public delegate void funcHolder();
    [HideInInspector] public funcHolder funcHolder_EffectFunc;

    [HideInInspector] public Dictionary<int, funcHolder> effectDictionary_Players = new Dictionary<int, funcHolder>();
    [HideInInspector] public Dictionary<int, funcHolder> effectDictionary_Enemies = new Dictionary<int, funcHolder>();
    
    // Basic variable initialization
    double Player_damageDealing = 0;
    double Player_extraDamage = 0;
    double Player_armorCreate = 0;
    double Player_healing = 0;
    int Player_cardsDrawing = 0;
    double Player_priorityInc = 0;
    double Player_extraPriorityCost = 0;
    
    double Enemy_damageDealing = 0;
    double Enemy_armorCreate = 0;
    double Enemy_priorityInc = 0;

    float ParticleDuration = 0;
    enum specialHandling { CastAt_playerEnd, CastAt_enemyEnd }

    // Struct: used for particle system
    public struct particleEffect
    {
        public GameObject particleObj;
        public float totalPlayTime;
        public int ID;
        public string effectName;
    }
    
    // Particle Prefabs Lists
    [Header("List of player particle prefabs")]
    public List<GameObject> playerParticlePrefabsList = new List<GameObject>();
    private List<particleEffect> playerParticlePrefabsPool = new List<particleEffect>();
    [Header("List of enemy particle prefabs")]
    public List<GameObject> enemyParticlePrefabsList = new List<GameObject>();
    private List<particleEffect> enemyParticlePrefabsPool = new List<particleEffect>();
    [Header("List of extra positioning for the particles")]
    public List<GameObject> ExtraPositioning = new List<GameObject>();
    
    [Header("Indicator Variables")]
    // Player Indicator
    public TMP_Text playerIndicatorText;
    [SerializeField] private GameObject playerIndicatorObj;
    private Animator _playerIndicatorController;

    // Enemy Indicator
    public TMP_Text enemyIndicatorText;
    [SerializeField] private GameObject enemyIndicatorObj;
    private Animator _enemyIndicatorController;

    private string damageTrigger = "Damage";
    private string healTrigger = "Heal";

    public void SetUp()
    {
        playerParticlePrefabsPool.Clear();
        enemyParticlePrefabsPool.Clear();
        playerObj = GameObject.Find("Player");
        enemyObj = GameObject.Find("Enemy");
        player = playerObj.GetComponent<Character>();
        enemy = enemyObj.GetComponent<Character>();
        
        // Assign variable GameObjects for the indicators
        playerIndicatorText = playerIndicatorObj.GetComponent<TMP_Text>();
        _playerIndicatorController = playerIndicatorObj.GetComponent<Animator>();
        enemyIndicatorText = enemyIndicatorObj.GetComponent<TMP_Text>();
        _enemyIndicatorController = enemyIndicatorObj.GetComponent<Animator>();
    }
    
    //=================================================================
    //                       Tagged Effect
    //-----------------------------------------------------------------
    // Note: Tagged effect function will be private only.
    private void DealDamage_ToTarget(Character target, double damageDealt)
    {
        // Check which target to use indicator for
        if (target == enemy)
        {
            enemyIndicatorText.text = "-" + damageDealt.ToString();
            _enemyIndicatorController.SetTrigger(damageTrigger);
        }
        else if (target == player)
        {
            playerIndicatorText.text = "-" + damageDealt.ToString();
            _playerIndicatorController.SetTrigger(damageTrigger);
        }

        // Check if the target has armor
        if (target.Armor_Current == 0)
        {
            target.Health_Current -= damageDealt;
        } else if (target.Armor_Current >= damageDealt)
        {
            target.Armor_Current -= damageDealt;
        } else if (target.Armor_Current < damageDealt)
        {
            target.Health_Current -= damageDealt - target.Armor_Current;
            target.Armor_Current = 0;
        }
    }
    
    private void DrawCards_Player(int cardAmount)
    {
        _script_DeckSystem.DrawMultipleCards(cardAmount);
    }

    private void CreateArmor_ToTarget(Character target, double armorAdded)
    {
        target.Armor_Current += armorAdded;
    }

    private void Banish_TheCard(Card_Basedata targetCard)
    {
        if(_script_DeckSystem.deckForCurrentBattle.Contains(targetCard))
        {
            _script_DeckSystem.deckForCurrentBattle.RemoveAt(_script_DeckSystem.deckForCurrentBattle.IndexOf(targetCard));
        }
    }

    private void Heal_ToTarget(Character target, double hpAdded)
    {
        // Variable to display health text
        double healthText = hpAdded;
        
        if ((target.Health_Current + hpAdded) > target.Health_Total)
        {
            healthText = target.Health_Total - target.Health_Current;
            target.Health_Current = target.Health_Total;
        }
        else
        {
            target.Health_Current += hpAdded;
        }
        
        // Check which target to use indicator for
        if (target == enemy)
        {
            enemyIndicatorText.text = "+" + healthText.ToString();
            _enemyIndicatorController.SetTrigger(healTrigger);
        }
        else if (target == player)
        {
            playerIndicatorText.text = "+" + healthText.ToString();
            _playerIndicatorController.SetTrigger(healTrigger);
        }
    }

    private void ReturnHand_Card(Card_Basedata targetCard)
    {
        _script_DeckSystem.activeCards.Insert(0, ReturnPool[ReturnPool.IndexOf(targetCard)]);
        _script_DeckSystem.DrawCardToHand();
    }



    // NOT IMPLEMENTED
    private void AddCardToDeck(){

    }

    private void PriorityIncrement(Character target, double cost)
    {
        // Increment priority
        _script_PrioritySystem.AddCost(target, cost);
        Character result = _script_PrioritySystem.GetNextTurnCharacter();
        if (result == player)
        {
            BattleController.instance.nextPhase = BattleController.TurnOrder.playerPhase;
            if (BattleController.instance.currentPhase == BattleController.TurnOrder.playerPhase)
            {
                BattleController.instance.lastPhase = BattleController.TurnOrder.playerPhase;
                BattleController.instance.currentPhase = BattleController.TurnOrder.playerEndPhase;
            }
            else
            {
                BattleController.instance.lastPhase = BattleController.TurnOrder.EnemyPhase;
                BattleController.instance.currentPhase = BattleController.TurnOrder.EnemyEndPhase;
            }
        }
        else if (result == enemy)
        {
            BattleController.instance.nextPhase = BattleController.TurnOrder.EnemyPhase;
            BattleController.instance.enableUsingCard = false;
            if (BattleController.instance.currentPhase == BattleController.TurnOrder.playerPhase)
            {
                BattleController.instance.lastPhase = BattleController.TurnOrder.playerPhase;
                BattleController.instance.currentPhase = BattleController.TurnOrder.playerEndPhase;
            }
            else
            {
                BattleController.instance.lastPhase = BattleController.TurnOrder.EnemyPhase;
                BattleController.instance.currentPhase = BattleController.TurnOrder.EnemyEndPhase;
            }
        }
    }
    
    // If it already exists, set it to active, and when the effect is played it will be set to disabled again
    // Object Pool with special position for the particle to spawn instead of using the player/enemy pos
    private void ParticleEvent(string effectName, int ID, float duration, GameObject overrideObj, bool playerEffect)
    {
        // Check which pool to use
        particleEffect foundEffect;
        if (playerEffect)
        {
            foundEffect = playerParticlePrefabsPool.Find(effect => effect.ID == ID);    
        }
        else
        {
            foundEffect = enemyParticlePrefabsPool.Find(effect => effect.ID == ID);    
        }
    
        // If the effect does not exist find it and setToActive
        if (foundEffect.particleObj == null)
        {
            // Instantiate it
            particleEffect newEffect = new particleEffect();

            // Check which particle list to use
            GameObject particleInstance;
            if (playerEffect)
            {
                particleInstance = playerParticlePrefabsList.Find(x => x.name == effectName);
            }
            else
            {
                particleInstance = enemyParticlePrefabsList.Find(x => x.name == effectName);
            }
            
            GameObject newParticle = Instantiate(particleInstance, overrideObj.transform.position, particleInstance.transform.rotation);
            // Store it in the pool
            newEffect.particleObj = newParticle;
            newEffect.totalPlayTime = duration;
            newEffect.ID = ID;
            newEffect.effectName = effectName;

            // Check which particle pool to add to
            if (playerEffect)
            {
                playerParticlePrefabsPool.Add(newEffect);
            }
            else
            {
                enemyParticlePrefabsPool.Add(newEffect);   
            }

            // Set object to deactivate after it's been played (object pool idea)
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                newEffect.particleObj.SetActive(false);
                BattleController.instance.enableCardActivation = true;
                TurnManipulator();
            }, newEffect.totalPlayTime ));
        }
        else
        {
            // If it exists in the pool then activate it and then set the effect to deActive
            foundEffect.particleObj.SetActive(true);
            // Set it to deActive after it has been played
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                // Debug.Log("test2");
                foundEffect.particleObj.SetActive(false);
                BattleController.instance.enableCardActivation = true;
                TurnManipulator();
            }, foundEffect.totalPlayTime ));
        }
    }

    void WithoutParticle(float soundDuration)
    {
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            TurnManipulator();
        }, soundDuration));
    }



    //for some unique particle, the turn will not changed
    void TurnManipulator()
    {
        BattleController.instance.enableTurnUpdate = true;
    }
    //-----------------------------------------------------------------
    //                      Tagged Effect Ends
    //=================================================================




    //=================================================================
    //                        Log System
    //-----------------------------------------------------------------

    [HideInInspector] public string cardName;
    [HideInInspector] public string descriptionLog;
    [SerializeField] GameObject Prefab_BattleLog;
    [SerializeField] GameObject contentHolder;
    [SerializeField] Scrollbar BattleLogScrollBar;
    [SerializeField] List<GameObject> BattleLogList = new();

    void ProcessLog(string character)
    {
        string BattleLog;
        string tempLog;
        GameObject instance = Instantiate(Prefab_BattleLog, contentHolder.transform);

        if (character == "player")
        {
            BattleLog = "<color=#3400fb>" + character + "</color>" + " casts " + cardName + ", costs " + Player_priorityInc + ". ";
            tempLog = "";
            if (Player_damageDealing != 0)
            {
                tempLog = "Deal <color=#f9303f>{0}</color> damage";
                string formattedText = string.Format(tempLog, Player_damageDealing);
                BattleLog = BattleLog + formattedText;
            }
            if (Player_armorCreate != 0)
            {
                tempLog = "Create <color=#9dc8f6>{0}</color> armors";
                string formattedText = string.Format(tempLog, Player_armorCreate);
                BattleLog = BattleLog + formattedText;
            }
            if (Player_cardsDrawing != 0)
            {
                tempLog = "Draw <color=#9dc8f6>{0}</color> cards";
                string formattedText = string.Format(tempLog, Player_cardsDrawing);
                BattleLog = BattleLog + formattedText;
            }
            if (Player_healing != 0)
            {
                tempLog = "Heal <color=#76f300>{0}</color> HP";
                string formattedText = string.Format(tempLog, Player_healing);
                BattleLog = BattleLog + formattedText;
            }

            if (descriptionLog != "")
            {
                tempLog = descriptionLog;
                BattleLog = BattleLog + " " + tempLog;
            }
        }
        else
        {
            BattleLog = "<color=#df0074>" + character + "</color>" + " casts " + cardName + ", costs " + Enemy_priorityInc +". ";
            tempLog = "";


            if (Enemy_damageDealing != 0)
            {
                tempLog = "Deal <color=#f9303f>{0}</color> damage";
                string formattedText = string.Format(tempLog, Enemy_damageDealing);
                BattleLog = BattleLog + formattedText;
            }
            if (Enemy_armorCreate != 0)
            {
                tempLog = "Create <color=#9dc8f6>{0}</color> armors";
                string formattedText = string.Format(tempLog, Enemy_armorCreate);
                BattleLog = BattleLog + formattedText;
            }
            if (descriptionLog != "")
            {
                tempLog = descriptionLog;
                BattleLog = BattleLog + " " + tempLog;
            }
        }


        instance.GetComponent<TMP_Text>().text = BattleLog;
        BattleLogList.Add(instance);
        descriptionLog = "";
        cardName = "";
    }
    private void LateUpdate()
    {
        BattleLogScrollBar.value = 0f;
    }






    //-----------------------------------------------------------------
    //                      SILVER CARDS
    //=================================================================
    // Draw 2 cards, cost 3
    public void ID1001_Payment()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        Player_cardsDrawing = 2;
        Manipulator_Player();
        
        // Play SFX
        PlaySound("sfx_Coin_Drop", 1);
        
        // Particle positioned under the player
        ParticleEvent("Payment", 1001, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // Deal 3 damage, cost 2
    public void ID1002_Whack()
    {
        ParticleDuration = 1.5f;
        Player_priorityInc = 2;
        Player_damageDealing = 3;
        Manipulator_Player();
        
        //Play SFX with delay
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        { 
            PlaySound("sfx_Stab", 1);
        }, 0.3f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        { 
            PlaySound("sfx_Swing", 1);
        }, 0.1f));

        // Particle positioned on the enemy
        ParticleEvent("Whack", 1002, ParticleDuration, ExtraPositioning[2], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // Gain 2 armor, cost 1
    public void ID1003_WhiteScales()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_armorCreate = 2;
        Manipulator_Player();
        
        // Play SFX
        PlaySound("sfx_Hiss", 1);
        
        // Particle positioned under the player
        ParticleEvent("WhiteScales", 1003, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // Draw 2 cards, gain 3 armors, cost 4
    public void ID1004_ShedSkin()
    {
        ParticleDuration = 5f;
        Player_priorityInc = 4;
        Player_armorCreate = 3;
        Player_cardsDrawing = 2;
        Manipulator_Player();

        // Play SFX
        PlaySound("sfx_Hiss", 1);

        // Particle positioned under the player
        ParticleEvent("ShedSkin", 1004, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    //-----------------------------------------------------------------
    //                      PURPLE CARDS
    //=================================================================
    // Deal 3 Damage, Banish this card, cost 1
    public void ID2001_ForbiddenVenom()
    {
        ParticleDuration = 2f;
        Player_priorityInc = 1;
        Player_damageDealing = 3;
        Manipulator_Player();
        
        // Play SFX with delay
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.3f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.6f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 1));
        
        // Play SFX
        SoundManager.PlaySound("sfx_Venom", 1);

        // Particle positioned on the enemy
        ParticleEvent("ForbiddenVenom", 2001, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2001));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // Deal 6 damage, the next card you play deal 4 more damage
    public void ID2002_SerpentCutlass()
    {
        ParticleDuration = 2f;
        Player_priorityInc = 5;
        Player_damageDealing = 6;
        Player_extraDamage = 4;
        Manipulator_Player();

        // Play SFX with delay
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.1f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.3f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.5f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.7f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Venom", 0.5f);
        }, 0.9f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Stab", 1f);
        }, 1.25f));
        
        // Play SFX
        PlaySound("sfx_Swing", 1);

        // Particle positioned on the enemy
        ParticleEvent("SerpentCutlass", 2002, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // Next card deal double damage
    public void ID2003_WisdomOfWisteria()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        isDealingDoubleDmg = true;
        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Wisdom", 0.2f);
        }, 1.1f));
        
        ParticleEvent("WisdomOfWisteria", 2003, ParticleDuration, ExtraPositioning[1], true);
        Manipulator_Player_Reset();
    }

    // Deal 1 damage, return card to hand
    public void ID2004_DemonFang()
    {
        ParticleDuration = 2f;
        Player_priorityInc = 1;
        Player_damageDealing = 1;
        Manipulator_Player();

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Crunch", 1);
            PlaySound("sfx_Venom", 0.3f);
        }, 0.3f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Crunch", 1);
            PlaySound("sfx_Venom", 0.3f);
        }, 0.6f));
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            PlaySound("sfx_Crunch", 1);
            PlaySound("sfx_Venom", 0.3f);
        }, 0.9f));
        SoundManager.PlaySound("sfx_Crunch", 1);
        
        // Particle positioned on the enemy
        ParticleEvent("DemonFang", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            ReturnHand_Card(ReturnPool.Find(cardBase => cardBase.ID == 2004));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // NOT IMPLEMENTED
    // If your deck has less than 10 cards, deal 6 damage
    public void ID2005_LastStand()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        int CardsinDeck = _script_DeckSystem.activeCards.Count();
        Player_damageDealing = 6;

        Manipulator_Player();



        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            if (CardsinDeck < 10)
            {
                DealDamage_ToTarget(enemy, Player_damageDealing);
            }
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));

    }

    // NOT IMPLEMENTED
    // deal x damage (x equals to the cards your banish in this battle times 2)
    public void ID2006_NoxiousRequiem()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        int BanishedCards = BanishPool.Count();
        Player_damageDealing = BanishedCards * 2;

        Manipulator_Player();



        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));

    }

    // NOT IMPLEMENTED
    // Draw 1 card. Banish this card. Add a Blood* to your hand.
    public void ID2007_BloodCrash()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_cardsDrawing = 1;

        Manipulator_Player();



        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2007));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));

    }

    // NOT IMPLEMENTED
    // deal 4 damage, if you health is lower than 10, deal 8 damage instead
    public void ID2008_FeintStrike()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_damageDealing = 4;

        if (player.Health_Current < 10)
        {
            Player_damageDealing = 8;
        }

        Manipulator_Player();



        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));

    }

    // NOT IMPLEMENTED
    // Env: everytime you banish a card, you draw a card
    public void ID2009_ToxicTorment()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_damageDealing = 6;

        Manipulator_Player();
       


        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));

    }
    
    // deal 6 damage to yourself, deal 12 damage
    public void ID2010_Savagery()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 5;
        Player_damageDealing = 12;
        Enemy_damageDealing = 6;

        Manipulator_Player();
       


        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            DealDamage_ToTarget(player, Enemy_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
     
    }
    
    
    // Damage yourself down to 1 HP. Deal that much damage.
    public void ID2011_CausticTrail()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 4;
        Player_damageDealing = player.Health_Current - 1;
        Enemy_damageDealing = Player_damageDealing;

        Manipulator_Player();
        

        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            player.Health_Current = 1;
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
     
    }
    
    // NOT IMPLEMENTED
    // Env: Whenever you deal damage to yourself, your next card deals +2 damage
    public void ID2012_VenomLace()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_damageDealing = 6;
        
        
        Manipulator_Player();
        


        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }
    
    
    // Deal 3 Damage. Gain +1 Max Health permanantly (continues on to next battles). Banish this card.
    public void ID2013_Siphon()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_damageDealing = 3;
        
        
        Manipulator_Player();
        


        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            player.Health_Total += 1;
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2013));

            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }
    
    // Deal 2 damage to yourself. Deal 1 damage to the enemy. Return
    public void ID2014_Ruination()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        Player_damageDealing = 1;
        Enemy_damageDealing = 2;

        Manipulator_Player();
        


        // ParticleEvent("", 2014, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            DealDamage_ToTarget(player, Enemy_damageDealing);
            ReturnHand_Card(ReturnPool.Find(cardBase => cardBase.ID == 2014));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }
    
    // NOT IMPLEMENTED
    // Env: Deal 2 damage to yourself at the start of your turn. -2 Priority
    public void ID2015_Intoxication()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_damageDealing = 6;

        Manipulator_Player();
    


        // ParticleEvent("", 2014, ParticleDuration, enemyObj, true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }
    
    //-----------------------------------------------------------------
    //                      GOLD CARDS
    //=================================================================
    // Draw 2
    public void ID3001_ForetoldFortune()
    {
        ParticleDuration = 6f;
        Player_priorityInc = 2;
        Player_cardsDrawing = 2;
        Manipulator_Player();

        // Play SFX
        PlaySound("sfx_Fortune", 1);
        
        // Particle positioned under the player
        ParticleEvent("ForetoldFortune", 3001, ParticleDuration, ExtraPositioning[0], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }
    
    // NOT IMPLEMENTED
    // Env: Every time you draw a card, deal one damage
    public void ID3002_TitansWrath()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        Player_damageDealing = 1;

        Manipulator_Player();
       

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // NOT IMPLEMENTED
    // Draw a card. It costs +1
    public void ID3003_Hongbao()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        Player_cardsDrawing = 1;

        Manipulator_Player();
       

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // IMPLEMENTED
    // If your hand has less than 3 cards, draw 4. Otherwise draw 2.
    public void ID3004_AssassinsTeapot()
    {
        ParticleDuration = 2.5f;
        Player_priorityInc = 3;
        int cardsInHand = _script_HandSystem.player_hands_holdCards.Count();
        if (cardsInHand < 3)
        {
            Player_cardsDrawing = 4;
        }
        else
        {
            Player_cardsDrawing = 2;
        }
        Manipulator_Player();

        // Play SFX
        PlaySound("sfx_Tea_Pour", 1);
        
        // Particle positioned under the player
        ParticleEvent("Assassin'sTeapot", 3004, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // NOT IMPLEMENTED
    // Every card you have played this combat costs one less for the rest of combat. Banish this card.
    public void ID3005_RedThread()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 4;
        
        Manipulator_Player();


        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 3005));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // IMPLEMENTED
    // Gain 2 Armor for each card you have in hand.
    public void ID3006_UnbreakingEmperor()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        int cardsInHand = _script_HandSystem.player_hands_holdCards.Count();
        Player_armorCreate = cardsInHand * 2;
        Manipulator_Player();

        // Particle positioned under the player
        ParticleEvent("UnbreakingEmperor", 3006, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();

        }, ParticleDuration / 2));
    }

    // NOT IMPLEMENTED
    // Env: At the start of your turn draw an additional card
    public void ID3007_FavoredFates()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_cardsDrawing = 1;

        Manipulator_Player();


        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // IMPLEMENTED
    // For each card in your hand, gain 4 Armor and deal 2 damage to the enemy
    public void ID3008_RoyalGambit()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 8;
        int cardsInHand = _script_HandSystem.player_hands_holdCards.Count();
        Player_armorCreate = 4 * cardsInHand;
        Player_damageDealing = 2 * cardsInHand;
        Manipulator_Player();
        
        // Particle positioned under the player
        ParticleEvent("RoyalGambit", 3008, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            CreateArmor_ToTarget(player, Player_armorCreate);
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // IMPLEMENTED
    // Draw cards until you have 4 cards , if you already have 4 cards gain 10 Armor
    public void ID3009_DeadlyDraw()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        int cardsInHand = _script_HandSystem.player_hands_holdCards.Count();
        Player_cardsDrawing = 0;
        Player_armorCreate = 0;
        if (cardsInHand < 4)
        {
            Player_cardsDrawing = 4 - cardsInHand;
        }
        else
        {
            Player_armorCreate = 10;
        }
        Manipulator_Player();

        WithoutParticle(ParticleDuration);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // IMPLEMENTED
    // Draw 2 cards and gain 6 Armor
    public void ID3010_CordCover()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 4;
        Player_armorCreate = 6;
        Player_cardsDrawing = 2;
        Manipulator_Player();

        WithoutParticle(ParticleDuration);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // IMPLEMENTED
    // Draw x amount of cards(x equals to cards in your hand)
    public void ID3011_BalancedBounty()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 5;
        int cardsInHand = _script_HandSystem.player_hands_holdCards.Count();
        Player_cardsDrawing = cardsInHand;
        Manipulator_Player();

        WithoutParticle(ParticleDuration);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // IMPLEMENTED
    // Double the Armor you have
    public void ID3012_UnbreakingGold()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        Player_armorCreate = player.Armor_Current;

        Manipulator_Player();


        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    //IMPLEMENTED
    // cost 8 Armor deal 6 damage (if you don't have enough armor, nothing will happen)
    public void ID3013_Terracotta()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        if(player.Armor_Current >= 8)
        {
            Player_armorCreate = -8;
            Player_damageDealing = 6;
        }

        Manipulator_Player();


        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            CreateArmor_ToTarget(player, Player_armorCreate);
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // IMPLEMENTED
    // draw 2 cards and deal x damage(x equals to the number of cards in your hand times 2)
    public void ID3014_LeechingTreasure()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 6;
        Player_cardsDrawing = 2;

        Manipulator_Player();


        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            int cardsInHand = _script_HandSystem.player_hands_holdCards.Count();
            Player_damageDealing = cardsInHand * 2;
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // IMPLEMENTED
    // If your Armor is less than 10 gain 12 Armor, otherwise gain 6 Armor
    public void ID3015_BronzeAge()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        if(player.Armor_Current < 10)
        {
            Player_armorCreate = 12;
        }
        else
        {
            Player_armorCreate = 6;
        }

        Manipulator_Player();

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            CreateArmor_ToTarget(player, Player_armorCreate);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Env: Every time you play a card, draw a card
    public void ID3016_MoneyTree()
    {        
        ParticleDuration = 3f;
        Player_priorityInc = 5;
        Player_cardsDrawing = 1;

        Manipulator_Player();

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }
    
    //-----------------------------------------------------------------
    //                      JADE CARDS
    //=================================================================
    // Heal 6
    public void ID4001_JadeSpirit()
    {
        ParticleDuration = 4f;
        Player_priorityInc = 2;
        Player_healing = 6;
        Manipulator_Player();
       
        PlaySound("sfx_Spirit", 1);

        // Particle positioned under the player
        ParticleEvent("JadeSpirit", 4001, ParticleDuration, ExtraPositioning[1], true);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
    }

    // NOT IMPLEMENTED
    // Env: All enemy cards cost 1 more
    public void ID4002_BrightKarma()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        Player_healing = 6;

        Manipulator_Player();
        
        WithoutParticle(ParticleDuration);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);

            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // Draw 2, Heal 4 Health
    public void ID4003_DauntlessDraw()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        Player_healing = 4;
        Player_cardsDrawing = 2;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            DrawCards_Player(Player_cardsDrawing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }

    // NOT IMPLEMENTED
    // Heal 3, Remove all negative status from yourself
    public void ID4004_LotusLeaf()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 1;
        Player_healing = 3;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Shuffle a random Sacred Herb* into your deck
    public void ID4005_HiddenGrotto()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }

    // NOT IMPLEMENTED
    // Remove all status cards from your deck and hand. Deal 3 damage per Status removed
    public void ID4006_QiBurst()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }


    // NOT IMPLEMENTED
    // Env: If you would heal more than your maximum hitpoints, instead deal 1 damage
    public void ID4007_JadeResolve()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Env: All healing becomes damage instead
    public void ID4008_MalechiteChain()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }

    // NOT IMPLEMENTED
    // Env: Every time you play a status card, heal 2
    public void ID4009_NurtuousNature()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // For each Herb card in your hand + deck, heal 2.
    public void ID4010_HerbalistBrew()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Until the end of turn, every time you heal enemy priority +1. Banish this card.
    public void ID4011_Triage()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
       
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Shuffle a Peril* into your deck
    public void ID4012_CauldronOfPeril()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }

    // NOT IMPLEMENTED
    // When you end your turn, your opponent gets +2 priority. Banish this card.
    public void ID4013_NephriteCurse()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Replace all status in your hand + deck with a random Peril*
    public void ID4014_JadePeril()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Replace all status in your hand + deck with a random Peril*
    public void ID4015_SealingStakes()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }


    // NOT IMPLEMENTED
    // You may take at most 5 damage until the start of your next turn.
    public void ID4016_Aegis()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    
    //-----------------------------------------------------------------
    //                    Not Implemented
    //=================================================================

    // NOT IMPLEMENTED
    // Your next card costs +1. Play this card as soon as it is drawn
    public void ID5001_Entangled()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // The next card you play is free
    public void ID5002_SacredHerb()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // The next card you play deals +3 damage
    public void ID5003_SacredHerb()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // The next card you play heals 2 and draws 1
    public void ID5004_SacredHerb()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }

    // Deal 4 Damage. Banish this card
    public void ID5005_PerilQiongQi()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        Player_damageDealing = 4;

        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(enemy, Player_damageDealing);
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 5005));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // Draw 3 cards. Banish this card
    public void ID5006_PerilTaoWu()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        Player_cardsDrawing = 3;
        
        Manipulator_Player();
        
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DrawCards_Player(Player_cardsDrawing);
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 5006));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }

    // NOT IMPLEMENTED
    // Your opponent skips their next turn. Banish this card.
    public void ID5007_PerilHundun()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        Player_healing = 6;

        Manipulator_Player();

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 5007));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Play this card once drawn. Take 3 self-damage. Banish this card.
    public void ID5008_PerilTaotie()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 0;
        Player_damageDealing = 3;

        Manipulator_Player();
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(player, Player_damageDealing);
            Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 5008));
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
        
    }

    // NOT IMPLEMENTED
    // Deal 3 damage to yourself. Draw one card. Banish this card. Add one Blood to your hand.
    public void ID5009_Blood()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 2;
        Player_healing = 6;

        Manipulator_Player();

        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Heal_ToTarget(player, Player_healing);
            Manipulator_Player_Reset();
        }, ParticleDuration / 2));
       
    }


    //=================================================================
    //                        ENEMY EFFECTS
    //-----------------------------------------------------------------
    // Deal 3 damage, cost 2
    public void Action_01_ThrowStone()
    {
        // Card Description
        ParticleDuration = 3f;
        Enemy_priorityInc = 2;
        Enemy_damageDealing = 3;
        cardName = "Throw Stone";
        descriptionLog = "";

        Manipulator_Enemy();
        
        // Play SFX with delay
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            SoundManager.PlaySound("sfx_Action_01_Throw_Stone", 0.5f);
        }, 1f));

        // Play SFX
        SoundManager.PlaySound("sfx_Action_01_Throw_Stone", 0.5f);
        
        // Particle positioned under the player
        ParticleEvent("ThrowStone", 1, ParticleDuration, ExtraPositioning[1], false);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(player, Enemy_damageDealing);
            Manipulator_Enemy_Reset();
        }, ParticleDuration / 2));
       
    }

    // Deal damage to player equal to his health, cost 2
    public void Action_02_BodySlam()
    {
        ParticleDuration = 2f;
        Enemy_priorityInc = 2;
        Enemy_damageDealing = enemy.Armor_Current;
        cardName = "Body Slam";
        descriptionLog = "Deal Damage equal to his current armor.";

        Manipulator_Enemy();

        // Play SFX
        SoundManager.PlaySound("sfx_Action_02_Body_Slam", 1);
        
        // Particle positioned on the player
        ParticleEvent("BodySlam", 2, ParticleDuration, ExtraPositioning[0], false);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DealDamage_ToTarget(player, Enemy_damageDealing);
            Manipulator_Enemy_Reset();
        }, ParticleDuration / 2));
        
    }
    // At the end of player turn, gain 2 armor
    public void Action_03_Stubborn()
    {
        ParticleDuration = 3f;
        Enemy_armorCreate = 1;
        cardName = "Stubborn";
        descriptionLog = "at the end of every player phase.";

        SoundManager.PlaySound("sfx_Action_03_Stubborn", 1);
        
        // Particle positioned under the enemy
        ParticleEvent("Stubborn", 3, ParticleDuration, ExtraPositioning[3], false);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            //manipulator not needed since this is static effect
            CreateArmor_ToTarget(enemy, Enemy_armorCreate);
            Manipulator_Enemy_Reset(specialHandling.CastAt_playerEnd);
        }, ParticleDuration / 2));
        
    }

    //-----------------------------------------------------------------
    //                       FOR ENEMY ENDS
    //=================================================================

    //=================================================================
    //                        Manipulator 
    //-----------------------------------------------------------------
    //Note manipulator must be called at the effect funtion after basic data is loaded
    //and reset must be called at the end of the effect function
    bool isDealingExtraDmg = false;
    bool isDealingDoubleDmg = false;
    bool isCostingExtraPriority = false;

    //<-----------Player-------------------------------------------------
    //this will be called for all player effect to check all flags
    void Manipulator_Player()
    {
        //disable card activation until the particle is played
        BattleController.instance.enableCardActivation = false;
        //to avoid overlap with turn change animation
        if(ParticleDuration < 2f)
        {
            ParticleDuration = 2f;
        }
        Manipulator_Player_DealingExtra();
        Manipulator_Player_CostExtra();
        Manipulator_Player_DealingDouble();
        PriorityIncrement(player, Player_priorityInc);
    }

    //this will be called for all player effect to turn off all flags
    void Manipulator_Player_Reset()
    {

        ProcessLog("Player");
        Player_damageDealing = 0;
        Player_priorityInc = 0;
        Player_cardsDrawing = 0;
        Player_armorCreate = 0;
        Player_healing = 0;
        ParticleDuration = 0;
    }

    //Helper func :  Next Card dealing extra
    void Manipulator_Player_DealingExtra()
    {
        //cards that apply extra damage
        //setup the extra damage and turn the flag to off
        if (isDealingExtraDmg && Player_damageDealing != 0)
        {
            Player_damageDealing += Player_extraDamage;
            isDealingExtraDmg = false;
            Player_damageDealing = 0;
        }
    }

    //Helper func :  Next Card dealing double
    void Manipulator_Player_DealingDouble()
    {
        //cards that apply extra damage
        //setup the extra damage and turn the flag to off
        if (isDealingDoubleDmg && Player_damageDealing != 0)
        {
            Player_damageDealing += Player_damageDealing;
            isDealingDoubleDmg = false;
        }
    }

    //Helper func :  Next Card costing more
    void Manipulator_Player_CostExtra()
    {
        if(isCostingExtraPriority && Player_extraPriorityCost != 0)
        {
            Player_priorityInc += Player_extraPriorityCost;
            isCostingExtraPriority = false;
            Player_extraPriorityCost = 0;
        }
       
    }
    
    //<-----------Enemy-------------------------------------------------
    void Manipulator_Enemy()
    {
        if (ParticleDuration < 2f)
        {
            ParticleDuration = 2f;
        }
        PriorityIncrement(enemy, Enemy_priorityInc);
    }

    void Manipulator_Enemy_Reset()
    {
        //enable turn change
        ProcessLog("Enemy");
        Enemy_damageDealing = 0;
        Enemy_priorityInc = 0;;
        Enemy_armorCreate = 0;
        ParticleDuration = 0;
    }

    void Manipulator_Enemy_Reset(specialHandling castTime)
    {
        if (castTime == specialHandling.CastAt_playerEnd)
        {
            ProcessLog("Enemy");
            Enemy_damageDealing = 0;
            Enemy_priorityInc = 0; ;
            Enemy_armorCreate = 0;
            ParticleDuration = 0;
        }
    }

    //=================================================================
    //                        Manipulator End
    //-----------------------------------------------------------------
    
    void Start()
    {
        // Silver Cards
        effectDictionary_Players.Add(1001, ID1001_Payment);
        effectDictionary_Players.Add(1002, ID1002_Whack);
        effectDictionary_Players.Add(1003, ID1003_WhiteScales);
        effectDictionary_Players.Add(1004, ID1004_ShedSkin);
        
        // Purple Cards
        effectDictionary_Players.Add(2001, ID2001_ForbiddenVenom);
        effectDictionary_Players.Add(2002, ID2002_SerpentCutlass);
        effectDictionary_Players.Add(2003, ID2003_WisdomOfWisteria);
        effectDictionary_Players.Add(2004, ID2004_DemonFang);
        effectDictionary_Players.Add(2005, ID2005_LastStand);
        effectDictionary_Players.Add(2006, ID2006_NoxiousRequiem);
        effectDictionary_Players.Add(2007, ID2007_BloodCrash);
        effectDictionary_Players.Add(2008, ID2008_FeintStrike);
        effectDictionary_Players.Add(2009, ID2009_ToxicTorment);
        effectDictionary_Players.Add(2010, ID2010_Savagery);
        effectDictionary_Players.Add(2011, ID2011_CausticTrail);
        effectDictionary_Players.Add(2012, ID2012_VenomLace);
        effectDictionary_Players.Add(2013, ID2013_Siphon);
        effectDictionary_Players.Add(2014, ID2014_Ruination);
        effectDictionary_Players.Add(2015, ID2015_Intoxication);
        
        // Gold Cards
        effectDictionary_Players.Add(3001, ID3001_ForetoldFortune);
        effectDictionary_Players.Add(3002, ID3002_TitansWrath);
        effectDictionary_Players.Add(3003, ID3003_Hongbao);
        effectDictionary_Players.Add(3004, ID3004_AssassinsTeapot);
        effectDictionary_Players.Add(3005, ID3005_RedThread);
        effectDictionary_Players.Add(3006, ID3006_UnbreakingEmperor);
        effectDictionary_Players.Add(3007, ID3007_FavoredFates);
        effectDictionary_Players.Add(3008, ID3008_RoyalGambit);
        effectDictionary_Players.Add(3009, ID3009_DeadlyDraw);
        effectDictionary_Players.Add(3010, ID3010_CordCover);
        effectDictionary_Players.Add(3011, ID3011_BalancedBounty);
        effectDictionary_Players.Add(3012, ID3012_UnbreakingGold);
        effectDictionary_Players.Add(3013, ID3013_Terracotta);
        effectDictionary_Players.Add(3014, ID3014_LeechingTreasure);
        effectDictionary_Players.Add(3015, ID3015_BronzeAge);
        effectDictionary_Players.Add(3016, ID3016_MoneyTree);
        
        // Jade Cards
        effectDictionary_Players.Add(4001, ID4001_JadeSpirit);
        effectDictionary_Players.Add(4002, ID4002_BrightKarma);
        effectDictionary_Players.Add(4003, ID4003_DauntlessDraw);
        effectDictionary_Players.Add(4004, ID4004_LotusLeaf);
        effectDictionary_Players.Add(4005, ID4005_HiddenGrotto);
        effectDictionary_Players.Add(4006, ID4006_QiBurst);
        effectDictionary_Players.Add(4007, ID4007_JadeResolve);
        effectDictionary_Players.Add(4008, ID4008_MalechiteChain);
        effectDictionary_Players.Add(4009, ID4009_NurtuousNature);
        effectDictionary_Players.Add(4010, ID4010_HerbalistBrew);
        effectDictionary_Players.Add(4011, ID4011_Triage);
        effectDictionary_Players.Add(4012, ID4012_CauldronOfPeril);
        effectDictionary_Players.Add(4013, ID4013_NephriteCurse);
        effectDictionary_Players.Add(4014, ID4014_JadePeril);
        effectDictionary_Players.Add(4015, ID4015_SealingStakes);
        effectDictionary_Players.Add(4016, ID4016_Aegis);

        // Status Cards
        effectDictionary_Players.Add(5001, ID5001_Entangled);
        effectDictionary_Players.Add(5002, ID5002_SacredHerb);
        effectDictionary_Players.Add(5003, ID5003_SacredHerb);
        effectDictionary_Players.Add(5004, ID5004_SacredHerb);
        effectDictionary_Players.Add(5005, ID5005_PerilQiongQi);
        effectDictionary_Players.Add(5006, ID5006_PerilTaoWu);
        effectDictionary_Players.Add(5007, ID5007_PerilHundun);
        effectDictionary_Players.Add(5008, ID5008_PerilTaotie);
        effectDictionary_Players.Add(5009, ID5009_Blood);



        // Enemy: Golem Cards
        effectDictionary_Enemies.Add(1, Action_01_ThrowStone);
        effectDictionary_Enemies.Add(2, Action_02_BodySlam);
        effectDictionary_Enemies.Add(3, Action_03_Stubborn);
    }
}
