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
    [SerializeField] TextMeshProUGUI PriorityText;

    // Armor Text Indicator
    public TMP_Text armorText;
    [SerializeField] private GameObject armorTextObj;
    
    // Armor Sprite
    [SerializeField] private GameObject armorSpriteObj;
    private Animator _armorSpriteController;
    private string _armorTrueTrigger = "ArmorTrue";
    private string _armorFalseTrigger = "ArmorFalse";
    public bool animArmor = false;

    // For health checking
    double HPChangedFrom;
    double lastFrameHP;
    double currentFrameHP;
    [SerializeField]float TotalHPMovingTime = 1f;
    [SerializeField] Gradient Gradient_HighHealth;
    [SerializeField] Gradient Gradient_MidHealth;
    [SerializeField] Gradient Gradient_LowHealth;
    [SerializeField] Gradient Gradient_CurrentInUse;
    
    void Start()
    {
        SetUp();
        StartCoroutine(UpdateHealthColor(1.5f, 1, 0));
    }

    // Update is called once per frame
    void Update()
    {
        DynamicHealthColorUpdate();
        CheckHPChange();
        animArmor = UpdateHealthAndShield(animArmor);
        UpdatePriorityText();
    }

    public void SetUp()
    {
        // Health variables
        _Text_HP = HealthText.GetComponent<TMP_Text>();
        HP_Bar = HealthBar.GetComponent<Image>();
        
        Gradient_CurrentInUse = Gradient_HighHealth;
        HP_Bar.color = Gradient_HighHealth.Evaluate(1f);
        CharacterName = CharacterData.characterName;
        descriptionMain = CharacterData.description_Main;
        Health_Total = CharacterData.Health_Total;
        Health_Current = CharacterData.Health_Current;
        Priority_Initial = CharacterData.Priority_Initial;
        Priority_Current = CharacterData.Priority_Current;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = CharacterData.characterSprite;
        
        // Armor variable
        armorText = armorTextObj.GetComponent<TMP_Text>();
        _armorSpriteController = armorSpriteObj.GetComponent<Animator>();
        Armor_Current = 0;
    }


    void CheckHPChange()
    {
        currentFrameHP = Health_Current;
        if (currentFrameHP != lastFrameHP)
        {
            StartCoroutine(UpdateHealth(TotalHPMovingTime, lastFrameHP, currentFrameHP));
            HPChangedFrom = lastFrameHP;
        }
    }

    void DynamicHealthColorUpdate()
    {
        double HealthRatio = Health_Current / Health_Total;
        if (HealthRatio >= 0.7)
        {
            Gradient_CurrentInUse = Gradient_HighHealth;
        }
        else if(HealthRatio >= 0.4)
        {
            Gradient_CurrentInUse = Gradient_MidHealth;
        }
        else
        {
            Gradient_CurrentInUse = Gradient_LowHealth;
        }
    }


    IEnumerator UpdateHealthColor(float TotalTime, double startColor, double endColor)
    {
        float totalTime = TotalTime;
        float elapsedTime = 0f;
        float timeRatio = 0;
        double startValue = startColor;
        double endValue = endColor;
        double value;
        bool repeatFlag = false;
        while (true)
        {
            timeRatio = elapsedTime / totalTime;
            value = Mathf.Lerp((float)startValue, (float)endValue, timeRatio);
            HP_Bar.color = Gradient_CurrentInUse.Evaluate((float)value);
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= totalTime && repeatFlag == false)
            {
                elapsedTime = 0;
                startValue = endColor;
                endValue = startColor;
                repeatFlag = true;
            }
            else if (elapsedTime >= totalTime && repeatFlag == true)
            {
                elapsedTime = 0;
                startValue = startColor;
                endValue = endColor;
                repeatFlag = false;
            }
            yield return null;
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
    
    bool UpdateHealthAndShield(bool armorAnim)
    {
        bool armorAnimPlayed = armorAnim;
        if (Armor_Current == 0)
        {
            if (armorAnim)
            {
                _armorSpriteController.SetTrigger(_armorFalseTrigger);
                armorAnimPlayed = false;
            }
            
            _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + "/" + System.Math.Round(Health_Total, 0).ToString();
            armorText.text = "";
        }
        else
        {
            if (armorAnim == false)
            {
                _armorSpriteController.SetTrigger(_armorTrueTrigger);
                armorAnimPlayed = true;
            }
            
            _Text_HP.text = System.Math.Round(Health_Current, 0).ToString() + "/" + System.Math.Round(Health_Total, 0).ToString();
            armorText.text = System.Math.Round(Armor_Current, 0).ToString();
        }
        lastFrameHP = Health_Current;
        return armorAnimPlayed;
    }

    void UpdatePriorityText() {
        PriorityText.text = Priority_Current.ToString();
    }
}
