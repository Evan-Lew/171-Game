using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDictionary : MonoBehaviour
{

    //move any extra caculation to the card effect fucntion for optimization.
    //make tagged function SIMPLY.
    //ez called from all other script without referencing
    //example:
    public static EffectDictionary instance;

    private void Awake()
    {
        instance = this;
 
    }

    [SerializeField] private DeckSystem _script_DeckSystem;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    Character player, enemy;
    [Header("list of banished cards")]
    public List<Card_Basedata> BanishPool;
    [Header("list of return cards")]
    public List<Card_Basedata> ReturnPool;

    [HideInInspector] public delegate void funcHolder();
    [HideInInspector] public funcHolder funcHolder_EffectFunc;

    [HideInInspector] public Dictionary<int, funcHolder> effectDictionary_Players = new Dictionary<int, funcHolder>();
    [HideInInspector] public Dictionary<int, funcHolder> effectDictionary_Enemies = new Dictionary<int, funcHolder>();



    //basic variable
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




    public void SetUp()
    {
        player = GameObject.Find("Player").GetComponent<Character>();
        enemy = GameObject.Find("Enemy").GetComponent<Character>();
    }


    //=================================================================
    //                       Tagged Effect
    //-----------------------------------------------------------------
    //Note: Tagged effect function will be private only.
    private void DealDamage_ToTarget(Character Target, double damageDealt)
    {
        //check if the target has armor
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
        //increment priority
        _script_PrioritySystem.AddCost(Target, Cost);
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
    //deal 4 damage, cost 2
    public void Action_01_Stomp()
    {
        Enemy_damageDealing = 4;
        Enemy_priorityInc = 2;
        Manipulator_Enemy();

        DealDamage_ToTarget(player, Enemy_damageDealing);
        PriorityIncrement(enemy, Enemy_priorityInc);

        Manipulator_Enemy_Reset();
    }

    //gain 4 armor, cost 2
    public void Action_10_Solidify()
    {
        Enemy_armorCreate = 4;
        Enemy_priorityInc = 2;
        Manipulator_Enemy();

        CreateArmor_ToTarget(enemy, Enemy_armorCreate);
        PriorityIncrement(enemy, Enemy_priorityInc);

        Manipulator_Enemy_Reset();
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


        effectDictionary_Enemies.Add(1, Action_01_Stomp);
        effectDictionary_Enemies.Add(10, Action_10_Solidify);
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
