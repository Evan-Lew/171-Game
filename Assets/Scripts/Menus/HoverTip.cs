using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI TipText;
    public RectTransform Window;
    public string message;
    private float wait = 0.6f;

    void Start()
    {
        TipText.text = default;
        Window.gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData data)  //hover
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData data)  //mouse leaves
    {
        StopAllCoroutines();
        Window.gameObject.SetActive(false);
        //HoverManager.LoseFocus();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(wait);
        ShowMessage();
    }
    private void ShowMessage()
    {
        TipText.text = message;
        Window.gameObject.SetActive(true);
        Window.transform.position = new Vector2(Input.mousePosition.x + 20, Input.mousePosition.y+25);
    }

}
