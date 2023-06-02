using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // For preloading
    public enum state { Handcard, DeckCandidate, DeckDisplay, None };
    public state cardState = state.None;
    public Card_Basedata.theme cardType;
    public Card_Basedata cardData;
    public string cardName;
    public string descriptionMain, descriptionLog;
    public int cardID;

    public int priorityCost;

    public TMP_Text _Text_Cost, _Text_DescriptionMain;
    public Image _Image_Card, _Image_Name;
    [Header("Don't change the list order")]
    [SerializeField] private List<GameObject> _CardTypeList;

    // Hovering
    [HideInInspector] public bool isInHand;
    public int handPosition;
    // Use to pair with hand manager
    private HandManager handManager;
    public Vector3 cardHoveringPosAdjustment;

    // Selecting
    [HideInInspector] public bool isSelected;
    private Collider theCollider;

    // Using Card and triggering effect
    private Character Enemy;

    // For card moving animation
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    // lerp between 0 to 1
    public float movingSpeed;
    public float rotateSpeed = 100f;

    // From battle controller
    private BattleController _script_BattleController;

    // From deck Edit System
    private DeckEditSystem _script_DeckEditSystem;
    
    private DeckSystem _script_DeckSystem;

    // Hovering
    bool enableOverEffect;
    [HideInInspector] public bool isHoveringAnimationCalled = false;
    Vector3 initializedScale;
    Vector3 targetScale;
    [HideInInspector] public float cardSizeChange_Lerp = 0.2f;

    // Candidate for Adding to Deck
    [HideInInspector] public int CandidatePosition;

    //private float delay = 2;
    float timer;

    void Awake()
    {
        loadObjects();
        loadCard();
    }

    void Start()
    {
        updateCard();
    }

    void Update()
    {
        if (cardState == Card.state.Handcard)
        {
            Actions_Handcards();
        }
        if (cardState == Card.state.DeckCandidate)
        {
            CardUtil_Movement();
        }
        if (cardState == Card.state.DeckDisplay)
        {

        }
        // Reset the mouse input bool
        justPressed = false;
        
        // // bug occured if cards were hovered over before they were fully drawn
        //
        // if (timer > delay)
        // {
        //     if (cardState == Card.state.Handcard)
        //     {
        //         Actions_Handcards();
        //     }
        //     if (cardState == Card.state.DeckCandidate)
        //     {
        //         CardUtil_Movement();
        //     }
        //     if (cardState == Card.state.DeckDisplay)
        //     {
        //
        //     }
        //     // Reset the mouse input bool
        //     justPressed = false;
        // } else {
        //     timer += Time.deltaTime;
        // }
    }

    public void loadObjects()
    {
        handManager = FindObjectOfType<HandManager>();
        theCollider = GetComponent<Collider>();
        enableOverEffect = true;
        _script_BattleController = GameObject.Find("Battle Controller").GetComponent<BattleController>();
        _script_DeckEditSystem = GameObject.Find("Deck Edit Manager").GetComponent<DeckEditSystem>();
        _script_DeckSystem = GameObject.Find("Deck System").GetComponent<DeckSystem>();
        if (cardState == state.Handcard)
        {
            Enemy = GameObject.Find("Enemy").GetComponent<Character>();
        }
    }

    // Load card from Card_Basedata
    public void loadCard()
    {
        cardType = cardData.cardColor;
        priorityCost = cardData.priorityCost;
        cardName = cardData.cardName;
        descriptionMain = cardData.description_Main;
        descriptionLog = cardData.description_Log;
        cardID = cardData.ID;
        initializedScale = transform.localScale;
        targetScale = initializedScale;
    }

    // Update the card
    public void updateCard()
    {
        setModel();
        _Text_Cost.text = priorityCost.ToString();
        _Text_DescriptionMain.text = descriptionMain.ToString();
        _Image_Card.sprite = cardData.Card_Front;
        _Image_Name.sprite = cardData.Card_Name;
    }

    // Move card to a point
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

        // Create a line where the mouse is, and use that line to check the collision
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
        // Left click after selecting the card. Trigger the card effect! and update the handcards
        // 0 left click and 1 right click
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, _Mask_AreaForCardsInteraction))
        {
            MoveToPoint(hit.point + new Vector3(0, 2f, 0f), Quaternion.identity);
        }

        // Selecting a card
        // 0 left click and 1 right click
        if (Input.GetMouseButtonDown(1))
        {
            ReturnToHand();
        }

        if (Input.GetMouseButtonDown(0) && !justPressed)
        {
            // Use the card if the area is correct, otherwise, return to the hand
            if (Physics.Raycast(ray, out hit, 100f, _Mask_AreaForCardsActivation))
            {
                if (_script_BattleController.enableCardActivation)
                {
                    EffectDictionary.instance.descriptionLog = descriptionLog;
                    EffectDictionary.instance.cardName = cardName;
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

    // Make the card return to the hand
    public void ReturnToHand()
    {
        isSelected = false;
        theCollider.enabled = true;
        MoveToPoint(handManager.player_hands_holdsCardsPositions[handPosition], handManager.minPos.rotation);
    }
    
    //======================================================
    //                  Mouse Action    
    //======================================================

    // Let the card move to the top when it's hovering
    private void OnMouseOver()
    {
        if (GameController.instance.enableMouseEffectOnCard)
        {
            if (cardState == Card.state.Handcard)
            {
                if (isInHand && enableOverEffect)
                {
                    // Find the card and rise it and move up
                    MoveToPoint(handManager.player_hands_holdsCardsPositions[handPosition] + cardHoveringPosAdjustment, transform.rotation);
                    ChangeToSize(initializedScale * 1.5f);
                    if (!isHoveringAnimationCalled)
                    {
                        handManager.MoveOtherCardAtHovering(this);
                        isHoveringAnimationCalled = true;
                    }
                }
            }
            else if (cardState == Card.state.DeckCandidate)
            {

            }
            enableOverEffect = false;
        }

    }

    private void OnMouseExit()
    {
        if (GameController.instance.enableMouseEffectOnCard)
        {
            enableOverEffect = true;
            if (cardState == Card.state.Handcard)
            {
                if (isInHand)
                {
                    // Find the card and rise it and move up
                    handManager.MoveOtherCardAtHovering_Reset();
                    ChangeToSize(initializedScale);
                    isHoveringAnimationCalled = false;
                }
            }
        }

    }

    private void OnMouseDown()
    {

        if (GameController.instance.enableMouseEffectOnCard)
        {
            SoundManager.PlaySound("sfx_Card_Pick", 0.25f);
            // Only activated in player turn
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
                if (!_script_DeckEditSystem.isCardPicked)
                {
                    _script_DeckEditSystem.AddCardToDeck(this.cardData);
                    _script_DeckEditSystem.DestroyCurrentCardsForPick();
                    _script_DeckEditSystem.UpdateText();
                    
                    // For deck editting
                    if (_script_DeckSystem.deckToUse.Count != 10)
                    {
                        DeckEditSystem.instance.SpawnCandidateForPick();
                    }
                }
            }
        }
    }

    //==============================================
    //         Helper Function for this script
    //==============================================
    // Select the card front for the card
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
