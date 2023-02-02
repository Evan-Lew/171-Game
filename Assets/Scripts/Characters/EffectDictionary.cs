using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDictionary : MonoBehaviour
{


    //ez called from all other script without referencing
    //example:
    public static EffectDictionary instance;
    private void Awake()
    {
        instance = this;
 
    }

    [SerializeField] private DeckSystem _script_DeckSystem;
    [SerializeField] private PrioritySystem _script_PrioritySystem;
    [SerializeField] Character player, enemy;

 
    public List<Card_Basedata> BanishPool;

    [HideInInspector] public delegate void funcHolder();
    [HideInInspector] public funcHolder funcHolder_EffectFunc;

    [HideInInspector] public Dictionary<int, funcHolder> effectDictionary_Players = new Dictionary<int, funcHolder>();
    [HideInInspector] public Dictionary<int, funcHolder> effectDictionary_Enemies = new Dictionary<int, funcHolder>();

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


    private void PriorityIncrement(Character Target, double Cost)
    {
        //increment priority
        _script_PrioritySystem.AddCost(Target, Cost);
        //update the meter/UI
        BattleController.instance.ProcessPriorityTurnControl();
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
        DrawCards_Player(2);
        PriorityIncrement(player, 3);
    }

    //deal 3 damage, cost 2
    public void ID1002_Whack()
    {
        DealDamage_ToTarget(enemy, 3);
        PriorityIncrement(player, 2);
    }

    //gain 2 armor, cost 2
    public void ID1003_WhiteScales()
    {
        CreateArmor_ToTarget(player, 2);
        PriorityIncrement(player, 1);
    }

    //draw 2 cards, gain 3 armors, cost 4
    public void ID1004_ShedSkin()
    {
        DrawCards_Player(2);
        CreateArmor_ToTarget(player, 3);
        PriorityIncrement(player, 4);
    }

    //draw 2 cards, gain 3 armors, cost 1
    public void ID2001_ForbiddenVenom()
    {
        DealDamage_ToTarget(enemy, 3);
        Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2001));
        PriorityIncrement(player, 1);
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
        DealDamage_ToTarget(player, 4);
        PriorityIncrement(enemy, 2);
    }

    //gain 4 armor, cost 2
    public void Action_10_Solidify()
    {
        CreateArmor_ToTarget(enemy, 4);
        PriorityIncrement(enemy, 2);
    }


    //-----------------------------------------------------------------
    //                       FOR ENEMY ENDS
    //=================================================================



    // testing the dictionary
    void Start()
    {
        effectDictionary_Players.Add(1001, ID1001_Payment);
        effectDictionary_Players.Add(1002, ID1002_Whack);
        effectDictionary_Players.Add(1003, ID1003_WhiteScales);
        effectDictionary_Players.Add(1004, ID1004_ShedSkin);
        effectDictionary_Players.Add(2001, ID2001_ForbiddenVenom);


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
