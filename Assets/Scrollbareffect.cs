using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scrollbareffect : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] private Transform battleLogSystem;
    //public float timeLerped = 1.0f;
    bool ifIsDisplayed = false;
    //float timeToLerp = 2;
    public Transform outside;
    public Transform inside;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //timeLerped += Time.deltaTime;
        if(ifIsDisplayed == false)
        {
            battleLogSystem.position = Vector3.Lerp(outside.position, inside.position, 1);
            ifIsDisplayed = true;
        }
        else
        {
            battleLogSystem.position = Vector3.Lerp(outside.position,outside.position, 0);
            ifIsDisplayed = false;
        }
    }
}
