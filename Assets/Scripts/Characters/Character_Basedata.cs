using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 2)]

public class Character_Basedata : ScriptableObject
{
    [Tooltip("Name of the Character")]
    public string characterName;

    [TextArea]
    public string description_Main;


    [Tooltip("Health of this Unit")]
    public float Health_Total;
    public float Health_Current;

}
