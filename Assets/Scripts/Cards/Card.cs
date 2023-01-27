using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public Card_Basedata cardData;

    public string cardName;
    public int priorityCost;
    public int damageDealt;
    public TMP_Text _Text_Cost, _Text_Name;



    // Start is called before the first frame update
    void Start()
    {
        loadCard();
        updateCard();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    //load card from Card_Basedata
    public void loadCard()
    {
        priorityCost = cardData.priorityCost;
        damageDealt = cardData.damageDealt;
        cardName = cardData.cardName;
    }


    public void updateCard()
    {
        _Text_Cost.text = priorityCost.ToString();
        _Text_Name.text = cardName.ToString();
    }


}
