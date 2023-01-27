using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]


public class Card : ScriptableObject
{
    [Tooltip("Name of the Card")]
    public string cardName;

    [TextArea]
    public string description_Main;


    [Tooltip("Priority cost of the Card")]
    public int priorityCost;


    public int damageDeal;

    public Sprite cardImage;
    public Sprite icon;

}
