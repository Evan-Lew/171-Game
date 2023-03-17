using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StarterDeck : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] StarterDeck_SO starterDeck;
    [HideInInspector] public Card_Basedata[] deck;
    Sprite DeckSprite;
    [SerializeField] GameObject descriptionObj;
    [SerializeField] TMP_Text textPopup;
    [HideInInspector]public bool enableTextDescription = false;

    private void Awake()
    {
        gameObject.GetComponent<Image>().sprite = starterDeck.DeckSprite;
        this.deck = starterDeck.deck;
        this.DeckSprite = starterDeck.DeckSprite;
        textPopup.text = starterDeck.description;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        enableTextDescription = true;
    }

  
 

    // Update is called once per frame
    void Update()
    {
        DisplayDescription();
    }


    void DisplayDescription()
    {
        if (enableTextDescription)
        {
            descriptionObj.SetActive(true);
        }
        else
        {
            descriptionObj.SetActive(false);
        }
    }



}
