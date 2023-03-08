using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scrollbareffect : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Area;
    public GameObject EndingPoint;
    bool IsDisplayed = false;


    //ray cast 在slider 里面关掉， 背景的Image ray cast 开起来
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsDisplayed)
        {
            Debug.Log("Hello");
            Area.SetActive(true);
            IsDisplayed = true;
        }

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Area.SetActive(false);
        IsDisplayed = false;

    }
}
