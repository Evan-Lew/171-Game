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


    //for health checking
    double HPChangedFrom;
    double lastFrameHP;
    double currentFrameHP;
    [SerializeField]float TotalHPMovingTime = 1f;


    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHPChange();
        UpdateHealthAndShield();
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


    void CheckHPChange()
    {
        currentFrameHP = Health_Current;
        if(currentFrameHP != lastFrameHP)
        {
            StartCoroutine(UpdateHealth(TotalHPMovingTime, lastFrameHP, currentFrameHP));
            HPChangedFrom = lastFrameHP;
        }
    }


    IEnumerator UpdateHealth(float TotalTime, double StartHP, double TargetHP)
    {
        float totalTime = TotalTime;
        float elapsedTime = 0f;
        float timeRatio = 0;
        double startValue = StartHP;
        double endValue = TargetHP;
        double value; 
        while (elapsedTime < totalTime)
        {
            timeRatio = elapsedTime / totalTime;
            value = Mathf.Lerp((float)startValue, (float)endValue, timeRatio);
            HP_Bar.fillAmount = (float)value / (float)Health_Total;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        HP_Bar.fillAmount = (float)Health_Current / (float)Health_Total;
    }


    void UpdateHealthAndShield()
    {
        if(Armor_Current == 0)
        {
            _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + " / " + System.Math.Round(Health_Total, 0).ToString();
        }
        else
        {
            _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + " / " + System.Math.Round(Health_Total, 0).ToString() + " + " + System.Math.Round(Armor_Current, 0).ToString();
        }
        lastFrameHP = Health_Current;
    }
}
