using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{

    public GameObject DoubleDamageBuff;
    public GameObject ExtraDamageBuff;

    private int shownBuffCount = 0;

    public void HideBuff(GameObject buffToHide){
        buffToHide.SetActive(false);
        shownBuffCount -= 1;
    }

    public void ShowBuff(GameObject buffToShow){
        shownBuffCount += 1;
        buffToShow.SetActive(true);
        buffToShow.transform.localPosition = new Vector3(shownBuffCount * 20, 0, 0);
    }

    public void hideDoubleDamage() {
        HideBuff(DoubleDamageBuff);
    }
        
    public void showDoubleDamage() {
        ShowBuff(DoubleDamageBuff);
    }

    public void hideExtraDamage() {
        HideBuff(ExtraDamageBuff);
    }

    public void showExtraDamage() {
        ShowBuff(ExtraDamageBuff);
    }
}
