using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrioritySystem : MonoBehaviour
{

    //initialize empty dictionary and starting priorities
    Dictionary <ScriptableObject, double> priorityDict = 
        new Dictionary <ScriptableObject, double> ();
    double initialPriority = 0.0;


    //Adds a character to the dictionary, catches error if already in dict
    public void AddCharacters(ScriptableObject character){
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
    public void AddCost(ScriptableObject character, double cost){

        priorityDict[character] = Math.Floor(priorityDict[character]);
        priorityDict[character] += cost;

        foreach (double c in priorityDict.Values){
            if (c == priorityDict[character]){
                priorityDict[character] += 0.1;
            }
        }
    }

    public void ResetPriority(){
        priorityDict.Clear();
        initialPriority = 0.0;
    }

    public ScriptableObject getNextTurnCharacter(){
        ScriptableObject nextChar = priorityDict.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
        return nextChar;
    }
}
