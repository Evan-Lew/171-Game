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
        foreach (var kvp in priorityDict)
        {
            if (kvp.Value == priorityDict[character] && kvp.Key != character)
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
