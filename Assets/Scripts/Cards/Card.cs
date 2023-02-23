using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Card : MonoBehaviour
{
    //for preloading
    public enum state { Handcard, DeckCandidate, DeckDisplay, None };
    public state cardState = state.None;
    public Card_Basedata.theme cardType;
    public Card_Basedata cardData;
    public string cardName;
    public string descriptionMain;
    public int cardID;

    public int priorityCost;
    //public Sprite card_sprite;


    public TMP_Text _Text_Cost, _Text_Name, _Text_DescriptionMain;
    public Image _Image_Card;
    [Header("Don't change the list order")]
    [SerializeField] private List<GameObject> _CardTypeList;

    //hovering
    [HideInInspector] public bool isInHand;
    public int handPosition;
    private HandManager handManager;            //use to pair with hand manager
    public Vector3 cardHoveringPosAdjustment;


    //Selecting
    [HideInInspector] public bool isSelected;
    private Collider theCollider;

    //Using Card and triggering effect
    private Character Enemy;


    //for card moving animation
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    //lerp between 0 to 1
    public float movingSpeed;
    public float rotateSpeed = 100f;


    //from battle control
    private BattleController _script_BattleController;

    //hovering
    bool enableOverEffect;
    [HideInInspector] public bool isHoveringAnimationCalled = false;
    Vector3 initializedScale;
    Vector3 targetScale;
    [HideInInspector] public float cardSizeChange_Lerp = 0.2f;

    //Candidate for Adding to Deck
    [HideInInspector] public int CandidatePosition;


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

        if(cardState == Card.state.DeckCandidate)
        {
            CardUtil_Movement();
        }
        //reset the mouse input bool
        justPressed = false;
    }




    public void loadObjects()
    {
        handManager = FindObjectOfType<HandManager>();
        theCollider = GetComponent<Collider>();
        enableOverEffect = true;
        _script_BattleController = GameObject.Find("Battle Controller").GetComponent<BattleController>();
        if (cardState == state.Handcard)
        {
            Enemy = GameObject.Find("Enemy").GetComponent<Character>();
        }


    }


    //load card from Card_Basedata
    public void loadCard()
    {
        cardType = cardData.cardColor;
        priorityCost = cardData.priorityCost;
        cardName = cardData.cardName;
        descriptionMain = cardData.description_Main;
        cardID = cardData.ID;
        initializedScale = transform.localScale;
        targetScale = initializedScale;
        //card_sprite = cardData.Card_Front;
        //_Image_Card = cardData.Card_Front;

    }

    //update the card
    public void updateCard()
    {
        setModel();
        _Text_Cost.text = priorityCost.ToString();
        _Text_Name.text = cardName.ToString();
        _Text_DescriptionMain.text = descriptionMain.ToString();
        _Image_Card.sprite = cardData.Card_Front;

        //_Material_CardFront.SetTexture("_Base_Texture", _Texture_CardType);

        //Debug.Log(gameObject.sGetTexture("_Base_Texture"));
    }

    //move card to a pont
    public void MoveToPoint(Vector3 pos_MoveTo, Quaternion rot_MoveTo)
    {
        targetPoint = pos_MoveTo;
        targetRotation = rot_MoveTo;
    }



    public void ChangeToSize(Vector3 target)
    {
        targetScale = target;
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

        CardUtil_Movement();

        //create a line where the mouse is, and use that line to check the collision
        if (isSelected)
        {
            selectAndUseACard();
        }


    }

    private void CardUtil_Movement()
    {

        transform.position = Vector3.Lerp(transform.position, targetPoint, movingSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 10f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, cardSizeChange_Lerp);
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
                if (_script_BattleController.enableCardActivation)
                {
                    ProcessCardEffect();
                    handManager.RemoveCardFromHand(this);
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                }

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

        if (cardState == Card.state.Handcard)
        {
            if (isInHand && enableOverEffect)
            {
                //find the card and rise it and move up
                MoveToPoint(handManager.player_hands_holdsCardsPositions[handPosition] + cardHoveringPosAdjustment, transform.rotation);
                ChangeToSize(initializedScale * 1.5f);
                if (!isHoveringAnimationCalled)
                {
                    handManager.MoveOtherCardAtHovering(this);
                    isHoveringAnimationCalled = true;
                }

            }
        }else if(cardState == Card.state.DeckCandidate)
        {
            Debug.Log("U are on me");
        }

        enableOverEffect = false;


    }

    private void OnMouseExit()
    {
        enableOverEffect = true;


        if (cardState == Card.state.Handcard)
        {
            if (isInHand)
            {

                //find the card and rise it and move up
                handManager.MoveOtherCardAtHovering_Reset();
                ChangeToSize(initializedScale);
                isHoveringAnimationCalled = false;
            }

        }

    }

    private void OnMouseDown()
    {
        //only activated in player turn

        if (cardState == Card.state.Handcard)
        {
            if (isInHand && _script_BattleController.enableUsingCard)
            {
                isSelected = true;
                theCollider.enabled = false;
                justPressed = true;
            }
        }
        else if (cardState == Card.state.DeckCandidate)
        {
            Debug.Log("u clicked");
        }

    }

    //==============================================
    //         Helper Function for this script
    //==============================================
    //select the card front for the card
    private void setModel()
    {

        if (cardData.cardColor == Card_Basedata.theme.White)
        {
            _CardTypeList[1].SetActive(true);

        }
        else if (cardData.cardColor == Card_Basedata.theme.Gold)
        {
            _CardTypeList[2].SetActive(true);
        }
        else if (cardData.cardColor == Card_Basedata.theme.Jade)
        {
            _CardTypeList[3].SetActive(true);
        }
        else if (cardData.cardColor == Card_Basedata.theme.Purple)
        {
            _CardTypeList[4].SetActive(true);
        }
        else
        {
            Debug.Log("Error: No card front material found");
            _CardTypeList[0].SetActive(true);

        }

    }


}
