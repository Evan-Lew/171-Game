using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scrollbareffect : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Area;
    public GameObject EndingPoint;
    bool IsDisplayed = false;


    //ray cast ��slider ����ص��� ������Image ray cast ������
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
