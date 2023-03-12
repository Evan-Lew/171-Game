using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class TutorialSetup : MonoBehaviour
{
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
        //run as long as the tutorial is not end
        if (!tutorialEnd)
        {
            //tutorial part 1 finish
            if (isPhase1Set && _HandManager.player_hands_holdCards.Count == 0)
            {
                if (!isPhase2Set)
                {
                    Phase_2_Setup();
                }
                else
                {
                    //tutorial part 2 finsih
                    if (_HandManager.player_hands_holdCards.Count == 0)
                    {
                        //tutorial part 3 finish
                        Phase_3_Setup();
                    }
                }
            }
        }

        //key for testing, for move the keytesting once the editiing is complete
        if (Input.GetKeyDown(KeyCode.L))
        {
            Phase_2_Setup();
        } else if (Input.GetKeyDown(KeyCode.K))
        {
            Phase_3_Setup();
        }
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
        _DeckSystem.DrawMultipleCardsThenStopDrawFeature(2);
        isPhase2Set = true;
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



}
