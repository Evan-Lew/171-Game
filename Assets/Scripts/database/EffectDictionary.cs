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
   
    public List<GameObject> ExtraPositioning = new List<GameObject>();
    
    public void SetUp()
    {
        playerParticlePrefabsPool.Clear();
        enemyParticlePrefabsPool.Clear();
        playerObj = GameObject.Find("Player");
        enemyObj = GameObject.Find("Enemy");
        player = playerObj.GetComponent<Character>();
        enemy = enemyObj.GetComponent<Character>();
    }
    
    //=================================================================
    //                       Tagged Effect
    //-----------------------------------------------------------------
    // Note: Tagged effect function will be private only.
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
                TurnManipulator(effectName);
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
                TurnManipulator(effectName);

            }, foundEffect.totalPlayTime ));
        }
    }

    //for some unique particle, the turn will not changed
    void TurnManipulator(string effectName)
    {
        if (effectName == "Stubborn")
        {
            BattleController.instance.enableTurnUpdate = true;
        }
        else
        {
            BattleController.instance.enableEndTurn = true;
            BattleController.instance.enableTurnUpdate = true;
        }
    }
    //-----------------------------------------------------------------
    //                      Tagged Effect Ends
    //=================================================================



    //=================================================================
    //                        PLAYER CARDS
    //-----------------------------------------------------------------
    
    //---SILVER CARDS---
    // Draw 2 cards, cost 3
    public void ID1001_Payment()
    {
        ParticleDuration = 3f;
        Player_cardsDrawing = 2;
        Player_priorityInc = 3;
        
        Manipulator_Player();
        DrawCards_Player(Player_cardsDrawing);
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned under the player
        ParticleEvent("Payment", 1001, ParticleDuration, ExtraPositioning[0], true);
        
        Manipulator_Player_Reset();
    }

    // Deal 3 damage, cost 2
    public void ID1002_Whack()
    {
        ParticleDuration = 2f;
        Player_damageDealing = 3;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned on the enemy
        // --WAITING FOR CARD IMPLEMENTATION--
        ParticleEvent("Whack", 1002, ParticleDuration, enemyObj, true);
        
        Manipulator_Player_Reset();
    }

    // Gain 2 armor, cost 1
    public void ID1003_WhiteScales()
    {
        ParticleDuration = 2f;
        Player_armorCreate = 2;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        CreateArmor_ToTarget(player, Player_armorCreate);
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned under the player
        ParticleEvent("WhiteScales", 1003, ParticleDuration, ExtraPositioning[0], true);
        
        Manipulator_Player_Reset();
    }

    // Draw 2 cards, gain 3 armors, cost 4
    public void ID1004_ShedSkin()
    {
        ParticleDuration = 2f;
        Player_armorCreate = 3;
        Player_cardsDrawing = 2;
        Player_priorityInc = 4;
        
        Manipulator_Player();
        DrawCards_Player(Player_cardsDrawing);
        CreateArmor_ToTarget(player, Player_armorCreate);
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned under the player
        ParticleEvent("ShedSkin", 1004, ParticleDuration, ExtraPositioning[0], true);
        
        Manipulator_Player_Reset();
    }

    //---PURPLE CARDS---
    // Draw 2 cards, gain 3 armors, cost 1
    public void ID2001_ForbiddenVenom()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 3;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        Banish_TheCard(BanishPool.Find(cardBase => cardBase.ID == 2001));
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned on the enemy
        ParticleEvent("ForbiddenVenom", 2001, ParticleDuration, enemyObj, true);
        
        Manipulator_Player_Reset();
    }

    // Deal 6 damage, the next card you play deal 4 more damage
    public void ID2002_SerpentCutlass()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 5;
        Player_extraDamage = 4;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned on the enemy
        ParticleEvent("SerpentCutlass", 2002, ParticleDuration, enemyObj, true);
        
        Manipulator_Player_Reset();
    }

    // Next card deal double damage
    public void ID2003_WisdomOfWisteria()
    {
        ParticleDuration = 3f;
        Player_priorityInc = 3;
        isDealingDoubleDmg = true;
        
        Manipulator_Player();
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // Deal 1 damage, return card to hand
    public void ID2004_DemonFang()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 1;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        ReturnHand_Card(ReturnPool.Find(cardBase => cardBase.ID == 2004));
        PriorityIncrement(player, Player_priorityInc);

        // Particle positioned on the enemy
        ParticleEvent("DemonFang", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // If your deck has less than 10 cards, deal 6 damage
    public void ID2005_LastStand()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // deal x damage (x equals to the cards your banish in this battle times 2)
    public void ID2006_NoxiousRequiem()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Draw 1 card. Banish this card. Add a Blood* to your hand.
    public void ID2007_BloodCrash()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // deal 4 damage, if you health is lower than 10, deal 8 damage instead
    public void ID2008_FeintStrike()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Env: everytime you banish a card, you draw a card
    public void ID2009_ToxicTorment()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // deal 6 damage to yourself, deal 12 damage
    public void ID2010_Savagery()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Damage yourself down to 1 HP. Deal that much damage.
    public void ID2011_CausticTrail()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Env: Whenever you deal damage to yourself, your next card deals +2 damage
    public void ID2012_VenomLace()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Deal 3 Damage. Gain +1 Max Health permanantly (continues on to next battles). Banish this card.
    public void ID2013_Siphon()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Deal 2 damage to yourself. Deal 1 damage to the enemy. Return
    public void ID2014_Ruination()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    
    // Env: Deal 2 damage to yourself at the start of your turn. -2 Priority
    public void ID2015_Intoxication()
    {
        ParticleDuration = 3f;
        Player_damageDealing = 6;
        Player_priorityInc = 1;
        
        Manipulator_Player();
        DealDamage_ToTarget(enemy, Player_damageDealing);
        PriorityIncrement(player, Player_priorityInc);
        
        // ParticleEvent("", 2004, ParticleDuration, enemyObj, true);
        Manipulator_Player_Reset();
    }
    

    //---GOLD CARDS---
    // Draw 2
    public void ID3001_FortoldFortune()
    {
        ParticleDuration = 3f;
        Player_cardsDrawing = 2;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        DrawCards_Player(Player_cardsDrawing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    //---JADE CARDS---
    // Heal 6
    public void ID4001_JadeSpirit()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Env: All enemy cards cost 1 more
    public void ID4002_Karma()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Draw 2, Heal 4 Health
    public void ID4003_DauntlessDraw()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Heal 3, Remove all negative status from yourself
    public void ID4004_LotusLeaf()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Shuffle a random Sacred Herb* into your deck
    public void ID4005_HiddenGrotto()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Remove all status cards from your deck and hand. Deal 3 damage per Status removed
    public void ID4006_QiBurst()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }


    // NOT IMPLEMENTED
    // Env: If you would heal more than your maximum hitpoints, instead deal 1 damage
    public void ID4007_JadeResolve()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Env: All healing becomes damage instead
    public void ID4008_MalechiteChain()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Env: Every time you play a status card, heal 2
    public void ID4009_NurtuousNature()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // For each Herb card in your hand + deck, heal 2.
    public void ID4010_HerbalistBrew()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Until the end of turn, every time you heal enemy priority +1. Banish this card.
    public void ID4011_Triage()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Shuffle a Peril* into your deck
    public void ID4012_CauldronOfPeril()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // When you end your turn, your opponent gets +2 priority. Banish this card.
    public void ID4013_NephriteCurse()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Replace all status in your hand + deck with a random Peril*
    public void ID4014_JadePeril()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }

    // NOT IMPLEMENTED
    // Replace all status in your hand + deck with a random Peril*
    public void ID4015_SealingStakes()
    {
        ParticleDuration = 3f;
        Player_healing = 6;
        Player_priorityInc = 2;
        
        Manipulator_Player();
        Heal_ToTarget(player, Player_healing);
        PriorityIncrement(player, Player_priorityInc);

        Manipulator_Player_Reset();
    }


    // NOT IMPLEMENTED
    // You may take at most 5 damage until the start of your next turn.
    public void ID4016_Aegis()
    {
        ParticleDuration = 3f;
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
    // Deal 3 damage, cost 2
    public void Action_01_ThrowStone()
    {
        // Card Description
        ParticleDuration = 3f;
        Enemy_damageDealing = 3;
        Enemy_priorityInc = 2;
        
        Manipulator_Enemy();
        DealDamage_ToTarget(player, Enemy_damageDealing);
        PriorityIncrement(enemy, Enemy_priorityInc);

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
        
        // Particle positioned under the player
        ParticleEvent("ThrowStone", 1, ParticleDuration, ExtraPositioning[0], false);
        
        Manipulator_Enemy_Reset();
    }

    // Deal damage to player equal to his health, cost 2
    public void Action_02_ThrowHimself()
    {
        ParticleDuration = 2f;
        Enemy_damageDealing = enemy.Armor_Current;
        Enemy_priorityInc = 2;
        
        Manipulator_Enemy();
        DealDamage_ToTarget(player, Enemy_damageDealing);
        PriorityIncrement(enemy, Enemy_priorityInc);

        // Play SFX
        SoundManager.PlaySound("sfx_Action_02_Throw_Himself", 1);
        
        // Particle positioned on the player
        ParticleEvent("ThrowHimself", 2, ParticleDuration, playerObj, false);
        
        Manipulator_Enemy_Reset();
    }
    // At the end of player turn, gain 2 armor
    public void Action_03_Stubborn()
    {
        ParticleDuration = 3f;
        Enemy_armorCreate = 1;
        
        Manipulator_Enemy();
        CreateArmor_ToTarget(enemy, Enemy_armorCreate);

        // Particle positioned under the enemy
        ParticleEvent("Stubborn", 3, ParticleDuration, ExtraPositioning[1], false);
        
        Manipulator_Enemy_Reset(specialHandling.CastAt_playerEnd);
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
        //to avoid overlap with turn change animation
        if(ParticleDuration < 2f)
        {
            ParticleDuration = 2f;
        }
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
    }

    void Manipulator_Enemy_Reset()
    {
        //enable turn change

        Enemy_damageDealing = 0;
        Enemy_priorityInc = 0;;
        Enemy_armorCreate = 0;
        ParticleDuration = 0;
    }

    void Manipulator_Enemy_Reset(specialHandling castTime)
    {
        if (castTime == specialHandling.CastAt_playerEnd)
        {
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
        effectDictionary_Players.Add(3001, ID3001_FortoldFortune);
        
        // Jade Cards
        effectDictionary_Players.Add(4001, ID4001_JadeSpirit);
        effectDictionary_Players.Add(4002, ID4002_Karma);
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


        // Enemy: Golem Cards
        effectDictionary_Enemies.Add(1, Action_01_ThrowStone);
        effectDictionary_Enemies.Add(2, Action_02_ThrowHimself);
        effectDictionary_Enemies.Add(3, Action_03_Stubborn);
    }
}
