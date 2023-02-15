using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;
using static CoroutineUtil;

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
    Character player, enemy;

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

    
    public float TurnsManagerFlag_RunTurnSwitchAfterSeconds = 0;
    public List<GameObject> ExtraPositioning = new List<GameObject>();


    public void SetUp()
    {
        playerParticlePrefabsPool.Clear();
        enemyParticlePrefabsPool.Clear();
        player = GameObject.Find("Player").GetComponent<Character>();
        enemy = GameObject.Find("Enemy").GetComponent<Character>();
    }
    
    //=================================================================
    //                       Tagged Effect
    //-----------------------------------------------------------------
    //Note: Tagged effect function will be private only.
    private void DealDamage_ToTarget(Character Target, double damageDealt)
    {
        // Check if the target has armor
        if(Target.Armor_Current == 0)
        {
            Target.Health_Current -= damageDealt;
        }else if(Target.Armor_Current >= damageDealt)
        {
            Target.Armor_Current -= damageDealt;
        }else if(Target.Armor_Current < damageDealt)
        {
            Target.Health_Current -= damageDealt - Target.Armor_Current;
            Target.Armor_Current = 0;
        }
    }
    
    private void DrawCards_Player(int cardAmount)
    {
        _script_DeckSystem.DrawMultipleCards(cardAmount);
    }

    private void CreateArmor_ToTarget(Character Target, double armorAdded)
    {
        Target.Armor_Current += armorAdded;
    }

    private void Banish_TheCard(Card_Basedata TargetCard)
    {
        if(_script_DeckSystem.deckForCurrentBattle.Contains(TargetCard))
        {
            _script_DeckSystem.deckForCurrentBattle.RemoveAt(_script_DeckSystem.deckForCurrentBattle.IndexOf(TargetCard));
        }
    }

    private void Heal_ToTarget(Character Target, double hpAdded)
    {
        if((Target.Health_Current + hpAdded) > Target.Health_Total)
        {
            Target.Health_Current = Target.Health_Total;
        }else
        {
            Target.Health_Current = Target.Health_Current + hpAdded;
        }
    }

    private void ReturnHand_Card(Card_Basedata TargetCard)
    {
        _script_DeckSystem.activeCards.Insert(0, ReturnPool[ReturnPool.IndexOf(TargetCard)]);
        _script_DeckSystem.DrawCardToHand();
    }

    private void PriorityIncrement(Character Target, double Cost)
    {
        // Increment priority
        _script_PrioritySystem.AddCost(Target, Cost);
    }
    
    // Object Pool: instantiate the particle gameObject prefab if it does not exist and disable it after effect is played
    // If it already exists, set it to active, and when the effect is played it will be set to disabled again
    // Particle will be played on the target (player or enemy)
    private void ParticleEvent(string effectName, int ID, float duration, Character target, bool playerEffect)
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
            
            GameObject newParticle = Instantiate(particleInstance, target.transform.position, particleInstance.transform.rotation);
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
                // Tell battle controller to run turn change
                TurnsManagerFlag_RunTurnSwitchAfterSeconds = newEffect.totalPlayTime;
            }, newEffect.totalPlayTime));
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
                TurnsManagerFlag_RunTurnSwitchAfterSeconds = foundEffect.totalPlayTime;
            }, foundEffect.totalPlayTime));
        }
    }
    
    // Object Pool with special position for the particle to spawn instead of using the player/enemy pos
    private void PositionedParticleEvent(string effectName, int ID, float duration, GameObject overrideObj, bool playerEffect)
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
                // Tell battle controller to run turn change
                TurnsManagerFlag_RunTurnSwitchAfterSeconds = newEffect.totalPlayTime;
            }, newEffect.totalPlayTime));
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
                TurnsManagerFlag_RunTurnSwitchAfterSeconds = foundEffect.totalPlayTime;
            }, foundEffect.totalPlayTime));
        }
    }

    //-----------------------------------------------------------------
    //                      Tagged Effect Ends
    //=================================================================



    //=================================================================
    //                        PLAYER CARDS
    //-----------------------------------------------------------------


    //draw 2 cards, cost 3
    public void ID1001_Payment()
    {
        Player_cardsDrawing = 2;
        Player_priorityInc = 3;
        Manipulator_Player();

        DrawCards_Player(Player_cardsDrawing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
        
        // Call the particle function
        //ParticleEvent("ThrowStone", 1, 4f, player);
        //extra postion player
        //PositionedParticleEvent("ThrowStone", 1, 3f, ExtraPositioning[0], true);
    }

    //deal 3 damage, cost 2
    public void ID1002_Whack()
    {
        Player_damageDealing = 3;
        Player_priorityInc = 2;
        Manipulator_Player();

        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //gain 2 armor, cost 2
    public void ID1003_WhiteScales()
    {
        Player_armorCreate = 2;
        Player_priorityInc = 1;
        Manipulator_Player();

        CreateArmor_ToTarget(player, Player_armorCreate);
        PriorityIncrement(player, Player_priorityInc);




        Manipulator_Player_Reset();
    }

    //draw 2 cards, gain 3 armors, cost 4
    public void ID1004_ShedSkin()
    {
        Player_armorCreate = 3;
        Player_cardsDrawing = 2;
        Player_priorityInc = 4;
        Manipulator_Player();

        DrawCards_Player(Player_cardsDrawing);
        CreateArmor_ToTarget(player, Player_armorCreate);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //draw 2 cards, gain 3 armors, cost 1
    public void ID2001_ForbiddenVenom()
    {
        Player_damageDealing = 3;
        Player_priorityInc = 1;
        Manipulator_Player();

        DealDamage_ToTarget(enemy, Player_damageDealing);
        Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2001));
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //deal 6 damage, the next card you play deal 4 more damage
    public void ID2002_SerpentCutlass()
    {
        Player_damageDealing = 6;
        Player_priorityInc = 5;
        Manipulator_Player();


        Player_extraDamage = 4;
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

        //next card deal double damage
    public void ID2003_WisdomOfWisteria()
    {
        Player_priorityInc = 3;
        Manipulator_Player();

        isDealingDoubleDmg = true;
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //deal 1 damage, return card to hand
    public void ID2004_DemonFang()
    {

        Player_damageDealing = 1;
        Player_priorityInc = 1;
        Manipulator_Player();

        DealDamage_ToTarget(enemy, Player_damageDealing);
        ReturnHand_Card(ReturnPool.Find(cardBase => cardBase.ID == 2004));
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //draw 2
    public void ID3001_FortoldFortune()
    {
        Player_cardsDrawing = 2;
        Player_priorityInc = 2;
        Manipulator_Player();

        DrawCards_Player(Player_cardsDrawing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //heal 6
    public void ID4001_JadeSpirit()
    {
        Player_healing = 6;
        Player_priorityInc = 2;
        Manipulator_Player();

        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }


    //-----------------------------------------------------------------
    //                      PLAYER CARDS EFFECTS
    //=================================================================



    //=================================================================
    //                        ENEMY EFFECTS
    //-----------------------------------------------------------------
    //deal 3 damage, cost 2
    public void Action_01_ThrowStone()
    {
        // Card Description
        Enemy_damageDealing = 3;
        Enemy_priorityInc = 2;
        Manipulator_Enemy();

        DealDamage_ToTarget(player, Enemy_damageDealing);
        PriorityIncrement(enemy, Enemy_priorityInc);

        Manipulator_Enemy_Reset();

        // Card SFX
        // Play SFX with delay
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            SoundManager.PlaySound("sfx_Action_01_ThrowStone1", 1);
            SoundManager.PlaySound("sfx_Action_01_ThrowStone2", 1);
        }, 1f));

        // Play SFX
        SoundManager.PlaySound("sfx_Action_01_ThrowStone1", 1);
        SoundManager.PlaySound("sfx_Action_01_ThrowStone2", 1);

        // Call the particle function
        //ParticleEvent("ThrowStone", 1, 4f, player);
        //extra postion player
        PositionedParticleEvent("ThrowStone", 1, 3f, ExtraPositioning[0], false);
    }

    //deal damage to player equal to his health, cost 2
    public void Action_02_ThrowHimself()
    {
        Enemy_damageDealing = enemy.Armor_Current;
        Enemy_priorityInc = 2;
        Manipulator_Enemy();

        DealDamage_ToTarget(player, Enemy_damageDealing);
        PriorityIncrement(enemy, Enemy_priorityInc);

        Manipulator_Enemy_Reset();
        
        // Play SFX
        SoundManager.PlaySound("sfx_Action_02_Throw_Himself", 1);
        
        ParticleEvent("ThrowHimself", 2, 1f, player, false);
    }

    //end of player turn, gain 2 armor
    public void Action_03_Stubborn()
    {
        Enemy_armorCreate = 1;
        Manipulator_Enemy();

        CreateArmor_ToTarget(enemy, Enemy_armorCreate);

        Manipulator_Enemy_Reset();
        //extra postion enemy
        PositionedParticleEvent("Stubborn", 3, 3f, ExtraPositioning[1], false);
    }

    ////deal 4 damage, cost 2
    //public void Action_01_Stomp()
    //{
    //    Enemy_damageDealing = 4;
    //    Enemy_priorityInc = 2;
    //    Manipulator_Enemy();

    //    DealDamage_ToTarget(player, Enemy_damageDealing);
    //    PriorityIncrement(enemy, Enemy_priorityInc);

    //    Manipulator_Enemy_Reset();
    //}

    ////gain 4 armor, cost 2
    //public void Action_10_Solidify()
    //{
    //    Enemy_armorCreate = 4;
    //    Enemy_priorityInc = 2;
    //    Manipulator_Enemy();

    //    CreateArmor_ToTarget(enemy, Enemy_armorCreate);
    //    PriorityIncrement(enemy, Enemy_priorityInc);

    //    Manipulator_Enemy_Reset();
    //}


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
        Manipulator_Player_DealingExtra();
        Manipulator_Player_CostExtra();
        Manipulator_Player_DealingDouble();
    }

    //this will be called for all player effect to turn off all flags
    void Manipulator_Player_Reset()
    {
        Player_damageDealing = 0;
        Player_priorityInc = 0;
        Player_cardsDrawing = 0;
        Player_armorCreate = 0;
        Player_healing = 0;
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

    }

    void Manipulator_Enemy_Reset()
    {
        Enemy_damageDealing = 0;
        Enemy_priorityInc = 0;;
        Enemy_armorCreate = 0;
    }

    //=================================================================
    //                        Manipulator End
    //-----------------------------------------------------------------




    void Start()
    {
        effectDictionary_Players.Add(1001, ID1001_Payment);
        effectDictionary_Players.Add(1002, ID1002_Whack);
        effectDictionary_Players.Add(1003, ID1003_WhiteScales);
        effectDictionary_Players.Add(1004, ID1004_ShedSkin);
        effectDictionary_Players.Add(2001, ID2001_ForbiddenVenom);
        effectDictionary_Players.Add(2002, ID2002_SerpentCutlass);
        effectDictionary_Players.Add(2003, ID2003_WisdomOfWisteria);
        effectDictionary_Players.Add(2004, ID2004_DemonFang);
        effectDictionary_Players.Add(3001, ID3001_FortoldFortune);
        effectDictionary_Players.Add(4001, ID4001_JadeSpirit);


        effectDictionary_Enemies.Add(1, Action_01_ThrowStone);
        effectDictionary_Enemies.Add(2, Action_02_ThrowHimself);
        effectDictionary_Enemies.Add(3, Action_03_Stubborn);
    }

    private void Update()
    {
        test();
    }


    void test()
    {
        int TestID;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestID = 1001;
            ID1001_Payment();
            Debug.Log("I am calling " + TestID);
        }


        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestID = 1002;
            ID1002_Whack();
            Debug.Log("I am calling " + TestID);
        }


        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestID = 1003;
            ID1003_WhiteScales();
            Debug.Log("I am calling " + TestID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestID = 1004;
            ID1004_ShedSkin();
            Debug.Log("I am calling " + TestID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestID = 1004;
            ID1004_ShedSkin();
            Debug.Log("I am calling " + TestID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestID = 1005;
            ID2001_ForbiddenVenom();
            Debug.Log("I am calling " + TestID);
        }
    }
}
