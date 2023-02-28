using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{

    public Character_Basedata CharacterData;
    public string CharacterName;
    public string descriptionMain;

    public TMP_Text _Text_HP;
    public Image HP_Bar;


    [Header("<Update Runtime>")]
    public double Health_Total;
    public double Health_Current;
    public double Priority_Initial;
    public double Priority_Current;

    public double Armor_Current;

    [SerializeField] GameObject HealthBar, HealthText;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        MonitorHPChange();
        updateHealthAndShield();
 
    }

    public void SetUp()
    {
        if (this.gameObject.name == "Player")
        {
            _Text_HP = HealthText.GetComponent<TMP_Text>();
            HP_Bar = HealthBar.GetComponent<Image>();
        }
        if (this.gameObject.name == "Enemy")
        {
            _Text_HP = HealthText.GetComponent<TMP_Text>();
            HP_Bar = HealthBar.GetComponent<Image>();
        }

        CharacterName = CharacterData.characterName;
        descriptionMain = CharacterData.description_Main;
        Health_Total = CharacterData.Health_Total;
        Health_Current = CharacterData.Health_Current;
        Priority_Initial = CharacterData.Priority_Initial;
        Priority_Current = CharacterData.Priority_Current;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = CharacterData.characterSprite;
        Armor_Current = 0;
    }


    double HPChangedFrom;
    double lastFrameHP;
    double currentFrameHP;
    bool isHPChanged = false;
    double HPChangeValue = 0.01f;
    double HPChangeSpeed ;
    double targetHPFillAmount;

    void MonitorHPChange()
    {
        //Debug.Log("Current : " + Health_Current + " Last: " + lastFrameHP);
        currentFrameHP = Health_Current;
        if(currentFrameHP != lastFrameHP)
        {
            if(currentFrameHP < lastFrameHP)
            {
                HPChangeSpeed = -HPChangeValue;
            }
            else
            {
                HPChangeSpeed = HPChangeValue;
            }
            isHPChanged = true;
            HPChangedFrom = lastFrameHP;
            targetHPFillAmount = (float)System.Math.Round(Health_Current, 0) / (float)System.Math.Round(Health_Total, 0);
        }
    }

    void UpdateHealth()
    {
        double currentValue = (HPChangedFrom + HPChangeSpeed);
        if(currentValue != Health_Current)
        {
            HP_Bar.fillAmount = (float)System.Math.Round(currentValue, 0) / (float)System.Math.Round(Health_Total, 0);
            HPChangedFrom = currentValue;
            Debug.Log("check");
        }

    }

    void updateHealthAndShield()
    {
        Debug.Log("I get called");
        if(Armor_Current == 0)
        {
            _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + " / " + System.Math.Round(Health_Total, 0).ToString();
            //HP_Bar.fillAmount = (float)System.Math.Round(Health_Current, 0) / (float)System.Math.Round(Health_Total, 0);
            UpdateHealth();
        }
        else
        {
            _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + " / " + System.Math.Round(Health_Total, 0).ToString() + " + " + System.Math.Round(Armor_Current, 0).ToString();
        }


        lastFrameHP = Health_Current;
    }
}
