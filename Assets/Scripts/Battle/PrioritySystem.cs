using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.EventSystems.EventTrigger;

public class PrioritySystem : MonoBehaviour
{

    //initialize empty dictionary and starting priorities
    public Dictionary <Character, double> priorityDict = new Dictionary <Character, double> ();
    double initialPriority;

    [SerializeField] Character player, enemy;

    private void Start()
    {
        //Player = 01;
        //Enemy = 02;
        //AddCharacters(player);
        //AddCharacters(enemy);

        //AddCost(player, 1);
        //AddCost(enemy, 0);

        //Debug.Log("return : " + getNextTurnCharacter());
        //AddCost(01, 1);

    }

    private void Update()
    {
        // AddCost(01, 1);
        // Helper_TestCostWithKey(KeyCode.P, Player, 1);


    }


    //Adds a character to the dictionary, catches error if already in dict
    public void AddCharacters(Character character){
        try
        {        
            priorityDict.Add(character, initialPriority);
            initialPriority += 0.1;
            }
        catch (ArgumentException)
        {
            Console.WriteLine("Key already in use!");
        }
    }

    //Adds priority cost to character in dictionary
    public void AddCost(Character character, double cost){

        priorityDict[character] = character.Priority_Current;
        priorityDict[character] = Math.Floor(priorityDict[character]);
        priorityDict[character] += cost;

        double temp_changeCost = priorityDict[character];
        foreach (double c in priorityDict.Values)
        {
            if (c == priorityDict[character])
            {
                temp_changeCost += 0.1;
                //something weird happened on next line 
                //priorityDict[character] = priorityDict[character] + 0.1;
                //Debug.Log(temp_changeCost);
            }
        }
        priorityDict[character] = temp_changeCost;
        character.Priority_Current = priorityDict[character];
    }

    public void ResetPriority(){
        priorityDict.Clear();
        initialPriority = 0.0;
    }



   
 

    public Character getNextTurnCharacter(){
      
        Character nextChar = priorityDict.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
        Debug.Log(nextChar.name + "turn starts.");
        return nextChar;
    }



    //==============================================
    //         Helper Function for this script
    //==============================================

    //used to test if the turn will be advanced
    void Helper_TestCostWithKey(KeyCode inputKey, Character character, double cost)
    {
        if (Input.GetKeyDown(inputKey))
        {
            AddCost(character, cost);
        }

    }
}
