using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Level03_Setup : MonoBehaviour
{
    [SerializeField] Character_Basedata[] enemies4Demo;
    [SerializeField] GameObject Canvas;
    // Start is called before the first frame update

    private void Awake()
    {

        Canvas.SetActive(true);
    }


    public void Button_InkGolem()
    {
        Canvas.SetActive(false);
        Character_Basedata enemy = enemies4Demo.Where(obj => obj.characterName == "Ink Golem").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.checkEnable = true;
    }

    public void Button_StoneRuishi()
    {
        Canvas.SetActive(false);
        Character_Basedata enemy = enemies4Demo.Where(obj => obj.characterName == "Stone Ruishi").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.checkEnable = true;
    }

    public void Button_Zhenniao()
    {
        Canvas.SetActive(false);
        Character_Basedata enemy = enemies4Demo.Where(obj => obj.characterName == "Zhenniao").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.checkEnable = true;

    }




}
