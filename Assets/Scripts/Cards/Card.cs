using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public enum state { Handcard, Deck, Others, None };
    public state cardState = state.None;

    public Card_Basedata cardData;
    public string cardName;
    public string descriptionMain;
    public int cardID;

    public int priorityCost;
    //public int damageDealt;



    //hovering
    [HideInInspector] public bool isInHand;
    public int handPosition;
    private HandManager handManager;            //use to pair with hand manager
    public Vector3 cardHoveringPosAdjustment = new Vector3(0f, 1f, 0.5f);


    //Selecting
    [HideInInspector] public bool isSelected;
    private Collider theCollider;

    //Using Card and triggering effect
    private Character Enemy;


    public TMP_Text _Text_Cost, _Text_Name, _Text_DescriptionMain;


    //for card moving animation
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    //lerp between 0 to 1
    [Range(0.0f, 1.0f)] public float cardMovingSpeed_Lerp = 0.2f;
    public float rotateSpeed = 100f;


    //from battle control
    private BattleController _script_BattleController;


    void Awake()
    {
        loadObjects();
        loadCard();
    }




    // Start is called before the first frame update
    void Start()
    {

        updateCard();

    }


    // Update is called once per frame
    void Update()
    {
        if (cardState == Card.state.Handcard)
        {
            Actions_Handcards();
        }

        //reset the mouse input bool
        justPressed = false;
    }




    public void loadObjects()
    {
        handManager = FindObjectOfType<HandManager>();
        theCollider = GetComponent<Collider>();
        Enemy = GameObject.Find("Enemy").GetComponent<Character>();
        _script_BattleController = GameObject.Find("Battle Controller").GetComponent<BattleController>();
    }


    //load card from Card_Basedata
    public void loadCard()
    {
        priorityCost = cardData.priorityCost;
        //damageDealt = cardData.damageDealt;
        cardName = cardData.cardName;
        descriptionMain = cardData.description_Main;
        cardID = cardData.ID;
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
        targetRotation = rot_MoveTo;
    }

    //======================================================
    //                  Cards Actions   
    //======================================================

    private bool justPressed;
    public LayerMask _Mask_AreaForCardsInteraction;
    public LayerMask _Mask_AreaForCardsActivation;
    //hands card action: select, play, hovering
    private void Actions_Handcards()
    {

        //let card move slowly
        transform.position = Vector3.Lerp(transform.position, targetPoint, cardMovingSpeed_Lerp);
        //let card rotate to sorted view quickly
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 10f);

        //create a line where the mouse is, and use that line to check the collision
        if (isSelected)
        {
            selectAndUseACard();
        }


    }


    private void selectAndUseACard()
    {
        //left click after selecting the card. Trigger the card effect! and update the handcards
        //0 left click  1 right click
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, _Mask_AreaForCardsInteraction))
        {


            MoveToPoint(hit.point + new Vector3(0, 2f, 0f), Quaternion.identity);
        }

        //selecting a card
        //0 left click  1 right click
        if (Input.GetMouseButtonDown(1))
        {

            ReturnToHand();
        }

        if (Input.GetMouseButtonDown(0) && !justPressed)
        {

            //use the card if the area is correct, otherwise, return to the hand
            if (Physics.Raycast(ray, out hit, 100f, _Mask_AreaForCardsActivation))
            {

                ProcessCardEffect();
                this.gameObject.SetActive(false);
                handManager.RemoveCardFromHand(this);
            }
            else
            {
                ReturnToHand();
            }

        }
    }

    void ProcessCardEffect()
    {
        EffectDictionary.instance.effectDictionary_Players[cardID]();
        BattleController.instance.ProcessPriorityTurnControl();
    }


    //make the card return to the hand
    public void ReturnToHand()
    {
        isSelected = false;
        theCollider.enabled = true;

        MoveToPoint(handManager.player_hands_holdsCardsPositions[handPosition], handManager.minPos.rotation);

    }



    //======================================================
    //                  Mouse Action    
    //======================================================

    //let card move top when it's hovering
    private void OnMouseOver()
    {
        if (isInHand)
        {
            //find the card and rise it and move up
            MoveToPoint(handManager.player_hands_holdsCardsPositions[handPosition] + cardHoveringPosAdjustment, this.transform.rotation);
        }


    }

    private void OnMouseExit()
    {
        if (isInHand)
        {
            //find the card and rise it and move up
            MoveToPoint(handManager.player_hands_holdsCardsPositions[handPosition], handManager.minPos.rotation);
        }
    }

    private void OnMouseDown()
    {
        //only activated in player turn
        if (isInHand && _script_BattleController.currentPhase == BattleController.TurnOrder.playerPhase)
        {
            isSelected = true;
            theCollider.enabled = false;
            justPressed = true;
        }
    }

    //==============================================
    //         Helper Function for this script
    //==============================================


}
