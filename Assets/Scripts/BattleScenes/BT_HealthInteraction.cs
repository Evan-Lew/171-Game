using UnityEngine;
using UnityEngine.EventSystems;

public class BT_HealthInteraction : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] GameObject display;
    [SerializeField] BT_Health healthscript;
    [SerializeField] LevelTransition _levelMove;
    [SerializeField] Character_Basedata current_player;
    Character player;
    GameObject playerObj;

    public void OnPointerExit(PointerEventData eventData)
    {
        healthscript.enableTextDescription = false;
    }

    public void Button_Cancel()
    {
        healthscript.enableTextDescription = false;
    }

    public void Button_Confirm()
    {
        BattleController.healthPlus = true;
        _levelMove.optionSelected = true;
    }
}
