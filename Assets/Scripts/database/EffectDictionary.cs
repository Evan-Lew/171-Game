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
        _script_PrioritySystem.AddCost(Target, Cost + nextcardcost);
        nextcardcost = 0;
        //BattleController.instance.ProcessPriorityTurnControl();
    }

    //-----------------------------------------------------------------
    //                      Tagged Effect Ends
    //=================================================================



    //=================================================================
    //                        PLAYER CARDS
    //-----------------------------------------------------------------


    bool isDoubleDamaged = false;
    //bool DoubleDamage()
    //{

    //}





    //draw 2 cards, cost 3
    public void ID1001_Payment()
    {
        DrawCards_Player(2);
        PriorityIncrement(player, 3);
    }

    //deal 3 damage, cost 2
    public void ID1002_Whack()
    {
        double damage = 3;
        if(doubledamage == true){
            damage = damage * 2;
            doubledamage = false;
        }
        DealDamage_ToTarget(enemy, damage + nextcarddeal);
        nextcarddeal = 0;
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
        double damage = 3;
        if(doubledamage == true){
            damage = damage * 2;
            doubledamage = false;
        }
        DealDamage_ToTarget(enemy, damage + nextcarddeal);
        nextcarddeal = 0;
        Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2001));
        PriorityIncrement(player, 1);
    }

    //deal 6 damage, the next card you play deal 4 more damage
    public void ID2002_SerpentCutlass()
    {
        double damage = 6;
        if(doubledamage == true){
            damage = damage * 2;
            doubledamage = false;
        }
        DealDamage_ToTarget(enemy, damage + nextcarddeal);
        nextcarddeal = 0;
        Next_Card_Deal(4);
        PriorityIncrement(player,5);
    }

        //next card deal double damage
    public void ID2003_WisdomOfWisteria()
    {
        Double_the_Damage();
        PriorityIncrement(player,3);
    }

    //deal 1 damage, return card to hand
    public void ID2004_DemonFang()
    {
        double damage = 1;
        if(doubledamage == true){
            damage = damage * 2;
            doubledamage = false;
        }
        DealDamage_ToTarget(enemy, damage + nextcarddeal);
        nextcarddeal = 0;
        ReturnHand_Card(ReturnPool.Find(cardBase => cardBase.ID == 2004));
        PriorityIncrement(player,1);
    }

    //draw 2
    public void ID3001_FortoldFortune()
    {
        DrawCards_Player(2);
        PriorityIncrement(player, 2);
    }

    //heal 6
    public void ID4001_JadeSpirit()
    {
        Heal_ToTarget(player, 6);
        PriorityIncrement(player, 2);
    }

    //next card cost 1 more
    public void ID5001_Burden()
    {
        Next_Card_Costmore(1);
    }


    //-----------------------------------------------------------------
    //                      PLAYER CARDS EFFECTS
    //=================================================================

    private double nextcarddeal = 0;
    private double nextcardcost = 0;
    private bool doubledamage = false;
    //return to hand effect

    private void Double_the_Damage()
    {
        doubledamage = true;
    }

    //next card deal more damage
    private void Next_Card_Deal(double damage)
    {
        nextcarddeal = damage;
    }

    //next card cost more
    private void Next_Card_Costmore(double cost)
    {
        nextcardcost = cost;
    }

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
        effectDictionary_Players.Add(2002, ID2002_SerpentCutlass);
        effectDictionary_Players.Add(2003, ID2003_WisdomOfWisteria);
        effectDictionary_Players.Add(2004, ID2004_DemonFang);
        effectDictionary_Players.Add(3001, ID3001_FortoldFortune);
        effectDictionary_Players.Add(4001, ID4001_JadeSpirit);
        effectDictionary_Players.Add(5001, ID5001_Burden);


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
