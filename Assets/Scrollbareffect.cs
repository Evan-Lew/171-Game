using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scrollbareffect : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Area;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Area.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Area.SetActive(false);
    }
}
