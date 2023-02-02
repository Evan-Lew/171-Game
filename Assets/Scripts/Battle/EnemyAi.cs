using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectDictionary;
using System.Linq;

public class EnemyAi : MonoBehaviour
{
    public Character_Basedata Enemy;
    

    public float amountOfTime ;
    [Header("Action# = index")]
    public List<Card_Basedata> Actions;
    List<int> attackPattern;
    int currentPatternIndex = 0;
    Dictionary<string, List<int>> enemyDictionary = new Dictionary<string, List<int>>();

    public TMP_Text _Text_Log;


    // Start is called before the first frame update
    void Start()
    {

        uploadEnemies();

        //Debug.Log(enemyDictionary["Laihong"][0]);
    }

    void uploadEnemies()
    {
        attackPattern = new List<int>() {1, 10};
        enemyDictionary.Add("Laihong", attackPattern);
    }


    public void EnemyUseAction()
    {

        if(currentPatternIndex < enemyDictionary[Enemy.name].Count)
        {
            EnemyEffect(attackPattern[currentPatternIndex]);
            currentPatternIndex++;
            Debug.Log(amountOfTime);
            StartCoroutine(waitLogReading(amountOfTime));
        }
        else
        {
            currentPatternIndex = 0;
            StartCoroutine(waitLogReading(amountOfTime));
            
        }
    }


    void EnemyEffect(int ActionID)
    {
        EffectDictionary.instance.effectDictionary_Enemies[ActionID]();
        
    }

    //after certain amount of time, the priority controller will calculate the turn result, and will automatically update the turn
    IEnumerator waitLogReading(float amountOfTime)
    {
        Debug.Log("into this");
        yield return new WaitForSeconds(amountOfTime);
        BattleController.instance.ProcessPriorityTurnControl();
        _Text_Log.text = "Next";
    }
}
