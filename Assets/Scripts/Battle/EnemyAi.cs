using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    //=======================================================
    //          AI Database Guide
    /*  1. Add Enemy to the Enemies by referencing from resource/template/enemy
     *  2. Update the Actions List in this script by referencing them from resource/template/card/enemy
     *  3. Code the the pattern designed for the enemy
     *  4. Add if loop if you want to trigger different pattern at different time
     *  5. Be careful with special handling for some cards, check with effectDictionary
     *  6. Don't change any other pattern/enemy made by other people, the person who add the enemy and pattern
     *      the list should take responsibility of making it works on times.
     * 
     *  NOTE: modify this script and EffectDictionary script ONLY!
     */
    //=======================================================

    [Header("Action# = index")]
    public List<Card_Basedata> Actions;
    
    [HideInInspector] public Dictionary<string, List<List<int>>> EnemyDictionary = new Dictionary<string, List<List<int>>>();
    List<int> attackPattern;
    List<List<int>> enemysPatterns;
    int currentPatternIndex = 0;

    [HideInInspector]public bool isActioned = false;
    
    private void Awake()
    {
        Load();
    }

    void Load()
    {
        Add_Golem();
        Add_Penghou();
        Add_Zhenniao();
        Add_StoneRuishi();
        Add_InkChimera();
        Add_XiaoQing();
        Add_FaHai();
    }

    void Add_Golem()
    {
        // Golem Pattern v.02
        // 1 -> (3) Deal 4 damage
        // 2 -> (4) Deal damage equal to armor
        // 3 -> (3) Gain 5 armor
        // 11 -> (6) Gain 11 armor
        // 13 -> (5) Next attack deals triple damage
        attackPattern = new List<int>() { 1, 3, 1, 3, 13, 1, 11, 2, 13, 1 };
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);

        EnemyDictionary.Add("Ink Golem", enemysPatterns);
    }

    void Add_Penghou()
    {
        // Add Penghou Pattern  Drain #4 -> Charge #5 -> Claw #14
        attackPattern = new List<int>() { 4, 14, 14, 14, 4, 14, 5 };
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Peng Hou", enemysPatterns);
    }

    // IMPLEMENTED
    void Add_Zhenniao(){
        // Zhenniao Pattern v.01
        // 1 -> (3) Deal 4 damage
        // 6 -> (7) Deal 5 Damage, Player's next card costs 3 more
        // 7 -> (5) Deal 7
        // 9 -> (3) Heal 1
        // 12 -> (7) Heal 7, Gain 7 Armor
        attackPattern = new List<int>() { 6, 7, 7, 6, 12, 9, 9, 12, 9, 9, 1};
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Zhenniao", enemysPatterns);
    }

    // IMPLEMENTED
    void Add_StoneRuishi()
    {
        attackPattern = new List<int>() { 10, 11, 10, 11 };
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        // attackPattern = new List<int>() { 13, 12};
        // enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Stone Rui Shi", enemysPatterns);
    }

    void Add_InkChimera()
    {
        // Chimera Pattern v.01
        // 1 -> (3) Deal 4 damage
        // 4 -> (5) Deal 3 damage and heal 3
        // 5 -> (4) Deal 7 Damage, Take 3 Damage
        attackPattern = new List<int>() { 5, 4, 1, 4, 1, 5, 4, 1, 1, 5, 4, 1, 1};
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Ink Chimera", enemysPatterns);
    }

    void Add_XiaoQing()
    {
        // Xiao Qing v.01
        // 1 -> (3) Deal 4 damage
        // 4 -> (5) Deal 3 damage and heal 3
        // 5 -> (4) Deal 7 Damage, Take 3 Damage
        attackPattern = new List<int>() { 5, 4, 1, 4, 1, 5, 4, 1, 1, 5, 4, 1, 1};
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Xiao Qing", enemysPatterns);
    }

    void Add_FaHai()
    {
        // Fa Hai v.01
        // 1 -> (3) Deal 4 damage
        // 4 -> (5) Deal 3 damage and heal 3
        // 5 -> (4) Deal 7 Damage, Take 3 Damage
        attackPattern = new List<int>() { 5, 4, 1, 4, 1, 5, 4, 1, 1, 5, 4, 1, 1};
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Fa Hai", enemysPatterns);
    }

    
    public void CastUniqueAbility_Golem()
    {
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
             EffectDictionary.instance.effectDictionary_Enemies[3]();
        }, 0.5f));
    }
    
    public void EnemyAction(string enemyName)
    {
        if (EnemyDictionary.ContainsKey(enemyName))
        {
            EnemyActionUtil(enemyName, GameController.instance.CharactersList[GameController.instance.CharactersList.FindIndex(x => x.name == enemyName)].Pattern);
        }
        else
        {
            Debug.Log("Error: EnemyAction(enemyName) in EnemyAi. No enemy found");
        }
    }

    private void EnemyActionUtil(string enemyName, Character_Basedata.PatternType Type)
    {
        if (Type == Character_Basedata.PatternType.single)
        {
            EnemyAction_SinglePattern(enemyName);
        }else if (Type == Character_Basedata.PatternType.multiple)
        {
            // Code the multiple pattern function and add it here.
        }
    }

    // Single pattern enemy
    public void EnemyAction_SinglePattern(string enemyName)
    {
        if (!isActioned)
        {
            isActioned = true;
            // Loop through the first pattern -> which is the only pattern for single pattern enemy
            if (currentPatternIndex < EnemyDictionary[enemyName][0].Count)
            {
                EnemyEffect(enemyName, EnemyDictionary[enemyName][0][currentPatternIndex]);
                // Go to next effect
                currentPatternIndex++;
            }
            else
            {
                //roll back to the index0 if the current pattern reach to the end
                currentPatternIndex = 0;
                EnemyEffect(enemyName, EnemyDictionary[enemyName][0][currentPatternIndex]);
            }
        }
    }

    void EnemyEffect(string enemyName,int actionID)
    {
        EffectDictionary.instance.effectDictionary_Enemies[actionID]();   
    }
}




