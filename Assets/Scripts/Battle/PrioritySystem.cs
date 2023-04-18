using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrioritySystem : MonoBehaviour
{
    // Initialize empty dictionary and starting priorities
    public Dictionary <Character, double> priorityDict = new Dictionary <Character, double> ();
    double initialPriority;
    Character playerKey;
    bool playerAdded = false;

    [SerializeField] PriorityBar priorityBar;

    // Adds a character to the dictionary, catches error if already in dict
    public void AddCharacters(Character character){
        try
        {
            // initialPriority = character.Priority_Current;
            initialPriority = 0;
            priorityDict.Add(character, initialPriority);

            if(!playerAdded){
                playerKey = character;
                playerAdded = true;
            }
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Key already in use!");
        }
    }

    // Adds priority cost to character in dictionary
    public void AddCost(Character character, double cost){

        priorityDict[character] = character.Priority_Current;
        priorityDict[character] = Math.Floor(priorityDict[character]);
        priorityDict[character] += cost;
        // double totalPriority = 0;
        double temp_changeCost = priorityDict[character];
        // foreach (var kvp in priorityDict)
        // {

        //     if (kvp.Value == priorityDict[character] && kvp.Key != character)
        //     {
        //         temp_changeCost += 0.5;
        //         //something weird happened on next line 
        //         //priorityDict[character] = priorityDict[character] + 0.1;
        //         //Debug.Log(temp_changeCost);
        //     }
        //     totalPriority += temp_changeCost;
        // }
        priorityDict[character] = temp_changeCost;
        character.Priority_Current = priorityDict[character];

        // double priorityDifference = priorityDict[playerKey] * 2 - totalPriority;
        // Debug.Log(priorityDict[playerKey]/totalPriority);
        // Debug.Log(priorityDict[playerKey]/totalPriority < 1-(priorityDict[playerKey]/totalPriority));
        // Debug.Log(priorityDict[playerKey]);
        // Debug.Log(priorityDifference);
        // Debug.Log(totalPriority);
        //priorityBar.moveBar();
    }

    public void ResetPriority(Character character){
        priorityDict[character] = 0;
    }

    public Character GetNextTurnCharacter(){

        Character nextChar = priorityDict.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
        return nextChar;
    }

    //==============================================
    //         Helper Function for this script
    //==============================================

    //used to test if the turn will be advanced
    //void Helper_TestCostWithKey(KeyCode inputKey, Character character, double cost)
    //{
    //    if (Input.GetKeyDown(inputKey))
    //    {
    //        AddCost(character, cost);
    //    }
    //}
}
