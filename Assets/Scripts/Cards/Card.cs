using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{

    public Card_Basedata cardData;
    public string cardName;
    public string descriptionMain;

    public int priorityCost;
    public int damageDealt;





    public TMP_Text _Text_Cost, _Text_Name, _Text_DescriptionMain;


    //for card moving animation
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    //lerp between 0 to 1
    public float cardMovingSpeed_Lerp = 0.3f;
    public float rotateSpeed = 100f;


    // Start is called before the first frame update
    void Start()
    {
        loadCard();
        updateCard();
    }


    // Update is called once per frame
    void Update()
    {
        //let card move slowly
        transform.position = Vector3.Lerp(transform.position, targetPoint, cardMovingSpeed_Lerp );
        //let card rotate to sorted view quickly
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    //load card from Card_Basedata
    public void loadCard()
    {
        priorityCost = cardData.priorityCost;
        damageDealt = cardData.damageDealt;
        cardName = cardData.cardName;
        descriptionMain = cardData.description_Main;
    }

    //update the card
    public void updateCard()
    {
        _Text_Cost.text = priorityCost.ToString();
        _Text_Name.text = cardName.ToString();
        _Text_DescriptionMain.text = descriptionMain.ToString();
    }

    //move card to a pont
    public void MoveToPoint(Vector3 pos_MoveTo, Quaternion rot_MoveTo)
    {
        targetPoint = pos_MoveTo;
        targetRotation= rot_MoveTo;
    }
}
