using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 2)]

public class Card_Basedata : ScriptableObject
{
    [Tooltip("Name of the Card")]
    public string cardName;

    [Tooltip("Unique ID of the Cards")]
    public int ID;

    [TextArea]
    public string description_Main;


    [Tooltip("Priority cost of the Card")]
    public int priorityCost;


    public int damageDealt;

    public Sprite cardImage;
    public Sprite icon;

}
