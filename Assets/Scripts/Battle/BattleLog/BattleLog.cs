using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : MonoBehaviour
{
    [Header("Battle Log")]
    [SerializeField] GameObject Prefab_BattleLog;
    [SerializeField] GameObject contentHolder;
    [SerializeField] Scrollbar BattleLogScrollBar;
    [SerializeField] Queue<GameObject> BattleLogQueue = new();
    //bool IsScrollBarIsBeingUsed = false;
    //bool setScrollBar2Bottom = false;

    public void Setup()
    {
       // BattleLogScrollBar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    public void Clear()
    {
        foreach (GameObject content in BattleLogQueue)
        {
            Destroy(content);
        }
        BattleLogQueue.Clear();
        //BattleLogScrollBar.onValueChanged.RemoveListener(OnScrollbarValueChanged);
    }


    public void ProcessLog(string character)
    {
        //setScrollBar2Bottom = true;
        string BattleLog;
        string tempLog;

        if (character == "player")
        {
            BattleLog = "<color=#3400fb>" + character + "</color>" + " casts " + EffectDictionary.instance.cardName + ", costs " + EffectDictionary.instance.Player_priorityInc + ". ";
            tempLog = "";
            if (EffectDictionary.instance.Player_damageDealing != 0)
            {
                tempLog = "Deal <color=#f9303f>{0}</color> damage";
                string formattedText = string.Format(tempLog, EffectDictionary.instance.Player_damageDealing);
                BattleLog = BattleLog + formattedText;
            }
            if (EffectDictionary.instance.Player_armorCreate != 0)
            {
                tempLog = "Create <color=#9dc8f6>{0}</color> armors";
                string formattedText = string.Format(tempLog, EffectDictionary.instance.Player_armorCreate);
                BattleLog = BattleLog + formattedText;
            }
            if (EffectDictionary.instance.Player_cardsDrawing != 0)
            {
                tempLog = "Draw <color=#9dc8f6>{0}</color> cards";
                string formattedText = string.Format(tempLog, EffectDictionary.instance.Player_cardsDrawing);
                BattleLog = BattleLog + formattedText;
            }
            if (EffectDictionary.instance.Player_healing != 0)
            {
                tempLog = "Heal <color=#76f300>{0}</color> HP";
                string formattedText = string.Format(tempLog, EffectDictionary.instance.Player_healing);
                BattleLog = BattleLog + formattedText;
            }

            if (EffectDictionary.instance.descriptionLog != "")
            {
                tempLog = EffectDictionary.instance.descriptionLog;
                BattleLog = BattleLog + " " + tempLog;
            }
        }
        else
        {
            BattleLog = "<color=#df0074>" + character + "</color>" + " casts " + EffectDictionary.instance.cardName + ", costs " + EffectDictionary.instance.Enemy_priorityInc + ". ";
            tempLog = "";


            if (EffectDictionary.instance.Enemy_damageDealing != 0)
            {
                tempLog = "Deal <color=#f9303f>{0}</color> damage";
                string formattedText = string.Format(tempLog, EffectDictionary.instance.Enemy_damageDealing);
                BattleLog = BattleLog + formattedText;
            }
            if (EffectDictionary.instance.Enemy_armorCreate != 0)
            {
                tempLog = "Create <color=#9dc8f6>{0}</color> armors";
                string formattedText = string.Format(tempLog, EffectDictionary.instance.Enemy_armorCreate);
                BattleLog = BattleLog + formattedText;
            }
            if (EffectDictionary.instance.descriptionLog != "")
            {
                tempLog = EffectDictionary.instance.descriptionLog;
                BattleLog = BattleLog + " " + tempLog;
            }
        }

        GameObject instance = Instantiate(Prefab_BattleLog, contentHolder.transform);
        instance.transform.SetAsFirstSibling();
        instance.GetComponent<TMP_Text>().text = BattleLog;
        BattleLogQueue.Enqueue(instance);
        EffectDictionary.instance.descriptionLog = "";
        EffectDictionary.instance.cardName = "";
    }

    //void OnScrollbarValueChanged(float value)
    //{
    //    if (Input.GetAxisRaw("Mouse ScrollWheel") == 0f && !IsScrollBarIsBeingUsed)
    //    {
    //        if (setScrollBar2Bottom)
    //        {
    //            SetScrollbarValueWithDelay();
    //            setScrollBar2Bottom = false;
    //        }
    //    }
    //}

    //IEnumerator SetScrollbarValueAtEndFrame()
    //{
    //    yield return new WaitForEndOfFrame();
    //    //BattleLogScrollBar.value = 0; 
    //}

    //// Call this method to set the scrollbar value with a delay
    //void SetScrollbarValueWithDelay()
    //{
    //    StartCoroutine(SetScrollbarValueAtEndFrame());
    //}

    //public void ScrollBarIsBeingUsed()
    //{
    //    IsScrollBarIsBeingUsed = true;
    //}

    //public void ScrollBarIsNotUsed()
    //{
    //    IsScrollBarIsBeingUsed = false;
    //}

}
