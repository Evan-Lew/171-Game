using UnityEngine;
using System.Linq;

public class PickDeckLevel_2Setup : MonoBehaviour
{
    [SerializeField] Character_Basedata[] enemiesForDemo;
    [SerializeField] GameObject Canvas;

    private void Awake()
    {
        GameController.instance.PickDeckLevel_2_Initialize();
        Canvas.SetActive(true);
    }
    
    public void Button_InkGolem()
    {
        Canvas.SetActive(false);
        Character_Basedata enemy = enemiesForDemo.Where(obj => obj.characterName == "Ink Golem").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.battleCondition = true;
    }

    public void Button_StoneRuiShi()
    {
        Canvas.SetActive(false);
        Character_Basedata enemy = enemiesForDemo.Where(obj => obj.characterName == "Stone Rui Shi").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.battleCondition = true;
    }

    public void Button_Zhenniao()
    {
        Canvas.SetActive(false);
        Character_Basedata enemy = enemiesForDemo.Where(obj => obj.characterName == "Zhenniao").SingleOrDefault();
        GameController.instance.StartTheBattle(enemy, true);
        GameController.instance.battleCondition = true;
    }
}
