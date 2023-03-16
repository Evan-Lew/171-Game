using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class TutorialSetup : MonoBehaviour
{
    [Header("Every Tutorial GameObject")]
    [SerializeField] GameObject tutorial01;
    [SerializeField] GameObject tutorial02;
    [SerializeField] GameObject tutorial03;
    
    [Header("Tutorial01 Pages")]
    [SerializeField] List<GameObject> tutorial01_Pages;
    
    [Header("Tutorial02 Pages")]
    [SerializeField] List<GameObject> tutorial02_Pages;
    
    [Header("Tutorial03 Pages")]
    [SerializeField] List<GameObject> tutorial03_Pages;

    [Header("Lists of Cards for the Tutorials")]
    [SerializeField] List<Card_Basedata> tutorialCards_P1;
    [SerializeField] List<Card_Basedata> tutorialCards_P2;
    [SerializeField] List<Card_Basedata> tutorialCards_P3;
    DeckSystem _DeckSystem;
    HandManager _HandManager;
    int Backup_StartDraw;

    bool isPhase1Set = false;
    bool isPhase2Set = false;
    bool tutorialEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        _DeckSystem = GameObject.Find("Deck System").GetComponent<DeckSystem>();
        _HandManager = GameObject.Find("Hand System").GetComponent<HandManager>();
        Backup_StartDraw = BattleController.instance.startingCardsAmount;
        Phase_1_Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // Run as long as the tutorial is not end
        if (!tutorialEnd)
        {
            // Tutorial part 1 finish
            if (isPhase1Set && _HandManager.player_hands_holdCards.Count == 0)
            {
                if (!isPhase2Set)
                {
                    Phase_2_Setup();
                }
                else
                {
                    // Tutorial part 2 finish
                    if (_HandManager.player_hands_holdCards.Count == 0)
                    {
                        // Tutorial part 3 finish
                        Phase_3_Setup();
                    }
                }
            }
        }

        //key for testing, for move the keytesting once the editiing is complete
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Phase_2_Setup();
        //} else if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Phase_3_Setup();
        //}
    }

    void Phase_1_Setup()
    {
        _DeckSystem.deckToUse.Clear();
        BattleController.instance.startDrawingCrads = false;
        foreach (Card_Basedata card in tutorialCards_P1)
        {
            _DeckSystem.deckToUse.Add(card);
        }

        GameController.instance.TutorialBattleSetup();
        isPhase1Set = true;

        _DeckSystem.DrawMultipleCardsThenStopDrawFeature(2);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Time.timeScale = 0;
            tutorial01.SetActive(true);
        }, 2.5f));
    }

    void Phase_2_Setup()
    {
        _DeckSystem.deckToUse.Clear();
        _DeckSystem.deckForCurrentBattle.Clear();
        _HandManager.Clear();
        foreach (Card_Basedata card in tutorialCards_P2)
        {
            _DeckSystem.deckToUse.Add(card);
            _DeckSystem.deckForCurrentBattle.Add(card);
        }

        _DeckSystem.DrawMultipleCardsThenStopDrawFeature(1);
        isPhase2Set = true;
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Time.timeScale = 0;
            tutorial02.SetActive(true);
        }, 9f));
    }

    void Phase_3_Setup()
    {
        _DeckSystem.enableDrawing = true;
        _DeckSystem.deckToUse.Clear();
        _DeckSystem.deckForCurrentBattle.Clear();
        _HandManager.Clear();
        foreach (Card_Basedata card in tutorialCards_P3)
        {
            _DeckSystem.deckToUse.Add(card);
            _DeckSystem.deckForCurrentBattle.Add(card);
        }

        _DeckSystem.DrawMultipleCards(3);
        tutorialEnd = true;
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            Time.timeScale = 0;
            tutorial03.SetActive(true);
        }, 11f));
    }

    void Backupdata()
    {
        BattleController.instance.startingCardsAmount = Backup_StartDraw;
        BattleController.instance.startDrawingCrads = true;
        _DeckSystem.enableDrawing = true;
    }

    private void OnDestroy()
    {
        Backupdata();
    }

    public void NextButton()
    {
        // Tutorial01
        if (tutorial01.activeSelf)
        {
            if (tutorial01_Pages[0].activeSelf)
            {
                tutorial01_Pages[0].SetActive(false);
                tutorial01_Pages[1].SetActive(true);
            }
        }

        // Tutorial02
        if (tutorial02.activeSelf)
        {
            if (tutorial02_Pages[0].activeSelf)
            {
                tutorial02_Pages[0].SetActive(false);
                tutorial02_Pages[1].SetActive(true);
            }
            else if (tutorial02_Pages[1].activeSelf)
            {
                tutorial02_Pages[1].SetActive(false);
                tutorial02_Pages[2].SetActive(true);
            }
        }
        
        // Tutorial03
        if (tutorial03.activeSelf)
        {
            if (tutorial03_Pages[0].activeSelf)
            {
                tutorial03_Pages[0].SetActive(false);
                tutorial03_Pages[1].SetActive(true);
            }
            else if (tutorial03_Pages[1].activeSelf)
            {
                tutorial03_Pages[1].SetActive(false);
                tutorial03_Pages[2].SetActive(true);
            }
            else if (tutorial03_Pages[2].activeSelf)
            {
                tutorial03_Pages[2].SetActive(false);
                tutorial03_Pages[3].SetActive(true);
            }
        }
    }

    public void PrevButton()
    {
        // Tutorial01
        if (tutorial01.activeSelf)
        {
            if (tutorial01_Pages[1].activeSelf)
            {
                tutorial01_Pages[1].SetActive(false);
                tutorial01_Pages[0].SetActive(true);
            }
        }
        
        // Tutorial02
        if (tutorial02.activeSelf)
        {
            if (tutorial02_Pages[1].activeSelf)
            {
                tutorial02_Pages[1].SetActive(false);
                tutorial02_Pages[0].SetActive(true);
            }
            else if (tutorial02_Pages[2].activeSelf)
            {
                tutorial02_Pages[2].SetActive(false);
                tutorial02_Pages[1].SetActive(true);
            }
        }
        
        // Tutorial03
        if (tutorial03.activeSelf)
        {
            if (tutorial03_Pages[1].activeSelf)
            {
                tutorial03_Pages[1].SetActive(false);
                tutorial03_Pages[0].SetActive(true);
            }
            else if (tutorial03_Pages[2].activeSelf)
            {
                tutorial03_Pages[2].SetActive(false);
                tutorial03_Pages[1].SetActive(true);
            }
            else if (tutorial03_Pages[3].activeSelf)
            {
                tutorial03_Pages[3].SetActive(false);
                tutorial03_Pages[2].SetActive(true);
            }
        }
    }
}
