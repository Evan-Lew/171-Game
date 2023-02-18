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
    public double Health_Total;
    public double Health_Current;

    [Tooltip("Priority of this Unit")]
    public double Priority_Initial;
    public double Priority_Current;


    [HideInInspector] public enum PatternType { single, multiple, special, player }
    public PatternType Pattern;
}
