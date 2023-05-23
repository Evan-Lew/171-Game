using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{

    public GameObject DoubleDamageBuff;
    public GameObject ExtraDamageBuff;

    private int shownBuffCount = 0;

    private bool doubleDamageShown = false;
    private bool extraDamageShown = false;


    public void HideBuff(GameObject buffToHide){
        buffToHide.SetActive(false);
        shownBuffCount -= 1;
    }

    public void ShowBuff(GameObject buffToShow){
        buffToShow.SetActive(true);
        buffToShow.transform.localPosition = new Vector3(shownBuffCount * 40, 0, 0);
        shownBuffCount += 1;
    }

    public void hideDoubleDamage() {
        if(doubleDamageShown){
            doubleDamageShown = false;
            HideBuff(DoubleDamageBuff);
        }
    }
        
    public void showDoubleDamage() {
        if (!doubleDamageShown){
            ShowBuff(DoubleDamageBuff);
            doubleDamageShown = true;
        }
    }

    public void hideExtraDamage() {
        if(extraDamageShown){        
            extraDamageShown = false;
            HideBuff(ExtraDamageBuff);
        }
    }

    public void showExtraDamage() {
        if (!extraDamageShown){
            ShowBuff(ExtraDamageBuff);
            extraDamageShown = true;
        }
    }
}
