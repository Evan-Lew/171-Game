using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectDictionary;

public class EnemyAi : MonoBehaviour
{
    //=======================================================
    //          AI Database Guide
    /*  1. Add Enmey to the Enemys by referencing from resource/template/ememy
     *  2. Update the Actions List in this script by referencing them from resource/template/card/enemy
     *  3. Code the the pattern designed for the enemy
     *  4. add if loop if you want to trigger different pattern at different time
     *  5. be careful with special handling for some cards, check with effectDictionary
     *  6. Don't change any other pattern/enemy made by other people, the person who add the emeny and pattern
     *      the list should take responsiblity of making it works on times.
     * 
     *  NOTE: modify this script and EffectDictionary script ONLY!
     */
    //=======================================================


    [Header("Enemy")]
    public List<Character_Basedata> Enemys;
    [Header("Action# = index")]
    public List<Card_Basedata> Actions;


    Dictionary<string, List<List<int>>> EnemyDictionary = new Dictionary<string, List<List<int>>>();
    List<int> attackPattern;
    List<List<int>> enemysPatterns;
    int currentPatternIndex = 0;
    public TMP_Text _Text_Log;

    [HideInInspector]public bool isActioned = false;



    private void Awake()
    {
        Load();
    }

    void Load()
    {
        Add_Golem();
    }

    void Add_Golem()
    {
        //Golem Pattern Throw Stone #1 -> Body Slam #2
        //Unique ability Stubborn #3 triggered at the end of player turn
        attackPattern = new List<int>() { 1, 2 };
        enemysPatterns = new List<List<int>>();
        enemysPatterns.Add(attackPattern);
        attackPattern = new List<int>() {3};
        enemysPatterns.Add(attackPattern);
        EnemyDictionary.Add("Golem", enemysPatterns);
    }

    public void CastUniqueAbility_Golem()
    {
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
             EffectDictionary.instance.effectDictionary_Enemies[3]();
             _Text_Log.text = "Golem doesn't know what he is doing. so <color=red>Armor + 1</color>";
        }, 0.5f));
        
    }
    

    public void EnemyAction(string enemyName)
    {
        if (EnemyDictionary.ContainsKey(enemyName))
        {
            
            EnemyActionUtil(enemyName, Enemys[Enemys.FindIndex(x => x.name == enemyName)].Pattern);
        }
        else
        {
            Debug.Log("Error: No enemy found");
        }
    }



    private void EnemyActionUtil(string enemyName, Character_Basedata.PatternType Type)
    {
        if(Type == Character_Basedata.PatternType.single)
        {
            EnemyAction_SinglePattern(enemyName);
        }else if(Type == Character_Basedata.PatternType.multiple)
        {
            //code the multiple pattern function and add it here.
        }

    }

    //single pattern enmey
    public void EnemyAction_SinglePattern(string enemyName)
    {
        if (!isActioned)
        {
            isActioned = true;
            //loop through the first pattern -> which is the only pattern for single pattern enemy
            if (currentPatternIndex < EnemyDictionary[enemyName][0].Count)
            {
                EnemyEffect(enemyName, EnemyDictionary[enemyName][0][currentPatternIndex]);
                //go to next effect
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

    void EnemyEffect(string enemyName,int ActionID)
    {
        EffectDictionary.instance.effectDictionary_Enemies[ActionID]();
        _Text_Log.text = enemyName + "\n" + "Using " + Actions[currentPatternIndex].name + "\n" + Actions[currentPatternIndex].description_Main;

    }
}




