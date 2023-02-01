using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.EventSystems.EventTrigger;

public class PrioritySystem : MonoBehaviour
{

    public int Enemy, Player;
    


    //initialize empty dictionary and starting priorities
    public Dictionary <int, double> priorityDict = new Dictionary <int, double> ();
    double initialPriority = 0.0;


    private void Start()
    {
        Player = 01;
        Enemy = 02;
        AddCharacters(Player);
        AddCharacters(Enemy);

        AddCost(01, 1);
        Debug.Log("Player:  " + priorityDict[Player]);
        Debug.Log("Enemy:  " + priorityDict[Enemy]);

    }

    private void Update()
    {
       // AddCost(01, 1);
        Helper_TestCostWithKey(KeyCode.P, Player, 1);
    }


    //Adds a character to the dictionary, catches error if already in dict
    public void AddCharacters(int character){
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
    public void AddCost(int character, double cost){

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

    }

    public void ResetPriority(){
        priorityDict.Clear();
        initialPriority = 0.0;
    }

    public int getNextTurnCharacter(){
        int nextChar = priorityDict.OrderBy(kvp => kvp.Value).FirstOrDefault().Key;
        return nextChar;
    }



    //==============================================
    //         Helper Function for this script
    //==============================================

    //used to test if the turn will be advanced
    void Helper_TestCostWithKey(KeyCode inputKey, int character, double cost)
    {
        if (Input.GetKeyDown(inputKey))
        {
            AddCost(character, cost);

            Debug.Log("Player:  " + priorityDict[Player]);
            Debug.Log("Enemy:  " + priorityDict[Enemy]);
        }


    }
}
