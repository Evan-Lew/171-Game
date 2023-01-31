using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;

public class Character : MonoBehaviour
{

    public Character_Basedata CharacterData;
    public string CharacterName;
    public string descriptionMain;

    public double Health_Total;
    public double Health_Current;

    public TMP_Text _Text_HP;

    // Start is called before the first frame update
    void Start()
    {
        CharacterName = CharacterData.characterName;
        descriptionMain = CharacterData.description_Main;
        Health_Total = CharacterData.Health_Total;
        Health_Current = CharacterData.Health_Current;
    }

    // Update is called once per frame
    void Update()
    {
        updateHealth();
    }


    void updateHealth()
    {
        _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + " / " + System.Math.Round(Health_Total, 0).ToString();
    }
}
