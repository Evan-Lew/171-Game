using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 2)]

public class Card_Basedata : ScriptableObject
{
    public enum theme { White, Gold, Jade, Purple, Enemy}
    public theme cardColor;

    [Tooltip("Name of the Card")]
    public string cardName;

    [Tooltip("Unique ID of the Cards")]
    public int ID;

    [TextArea]
    public string description_Main;


    [Tooltip("Priority cost of the Card")]
    public int priorityCost;
    //public int NumInHand;

    //public int damageDealt;

    public Sprite Card_Front;
    public Sprite icon;



}
