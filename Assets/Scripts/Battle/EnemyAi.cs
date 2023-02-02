using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectDictionary;
using System.Linq;

public class EnemyAi : MonoBehaviour
{
    public Character_Basedata Enemy;
    

    public float amountOfTimeWaiting ;
    [Header("Action# = index")]
    public List<Card_Basedata> Actions;
    List<int> attackPattern;
    int currentPatternIndex = 0;
    Dictionary<string, List<int>> enemyDictionary = new Dictionary<string, List<int>>();

    public TMP_Text _Text_Log;

    bool isActioned = false;

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
        if (!isActioned)
        {
            isActioned = true;
            if (currentPatternIndex < enemyDictionary[Enemy.name].Count)
            {
                EnemyEffect(attackPattern[currentPatternIndex]);
                currentPatternIndex++;
                StartCoroutine(waitLogReading(amountOfTimeWaiting));
            }
            else
            {
                currentPatternIndex = 0;
                EnemyEffect(attackPattern[currentPatternIndex]);
                StartCoroutine(waitLogReading(amountOfTimeWaiting));

            }
        }
    }


    void EnemyEffect(int ActionID)
    {
        EffectDictionary.instance.effectDictionary_Enemies[ActionID]();
        _Text_Log.text = Enemy.name + "\n" + "Using " + Actions[currentPatternIndex].name + "\n" + Actions[currentPatternIndex].description_Main;


    }

    //after certain amount of time, the priority controller will calculate the turn result, and will automatically update the turn
    IEnumerator waitLogReading(float amountOfTime)
    {
        yield return new WaitForSeconds(amountOfTime);
        BattleController.instance.ProcessPriorityTurnControl();
        _Text_Log.text = "Next";
        isActioned = false;

    }
}
