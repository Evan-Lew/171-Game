using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSetup : MonoBehaviour
{
    [SerializeField] GameObject introTextBox;
    [SerializeField] GameObject introTextManager;
    [SerializeField] GameObject outroTextBox;
    [SerializeField] GameObject outroTextManager;
    
    [Header("Every Tutorial GameObject")]
    [SerializeField] GameObject tutorial01;
    [SerializeField] GameObject tutorial02;
    [SerializeField] GameObject tutorial03;
    [SerializeField] GameObject historyTutorial;
    
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
    private bool _historyTutorialStarted = false;

    public bool levelEnd = false;
    
    private bool _dialoguePlaying = true;
    private bool _introDialoguePlayed = false;

    void Start()
    {
        GameController.instance.StartDialogue();
        GameController.instance.tutorialIntroDialoguePlaying = true;
        GameController.instance.tutorialOutroDialoguePlaying = true;
        
        // Time delay to activate the dialogue text box
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            GameController.instance.CharacterTalking("rightIsTalking", true);
            introTextBox.SetActive(true);
            introTextManager.SetActive(true);
        }, 4f));
    }

    void Update()
    {
        HighlightCharacterTalking();
        
        // If the tutorial dialogue intro is over then start the battle 
        if (_introDialoguePlayed == false && GameController.instance.tutorialIntroDialoguePlaying == false)
        {
            _introDialoguePlayed = true;
            introTextBox.SetActive(false);

            // Time delay to start the music
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                SoundManager.PlaySound("bgm_Mountain_Of_Myths", 0.1f);
            
            }, 3f));
        
            // Time delay to start the battle
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                _DeckSystem = GameObject.Find("Deck System").GetComponent<DeckSystem>();
                _HandManager = GameObject.Find("Hand System").GetComponent<HandManager>();
                Backup_StartDraw = BattleController.instance.startingCardsAmount;
                Phase_1_Setup();
                _dialoguePlaying = false;
            }, 4.5f));
        }
        
        if (_dialoguePlaying == false)
        {
            Tutorials();
            LevelManagement();

            if (tutorial01.activeSelf || tutorial02.activeSelf  || tutorial03.activeSelf)
            {
                GameController.instance.enableMouseEffectOnCard = false;
            }
            else
            {
                GameController.instance.enableMouseEffectOnCard = true;
            }    
        }

        // Check if the outro text is over
        if (GameController.instance.tutorialOutroDialoguePlaying == false)
        {
            GameController.instance.CharacterTalking("rightIsTalking", false);
            outroTextBox.SetActive(false);
            GameController.instance.FadeOut();
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                SceneManager.LoadScene("MainMenu");
            }, 6f));    
        }
    }
    
    // Helper Function: highlight characters when talking (this is hardcoded to match the length of the TextManager's sentences)
    public void HighlightCharacterTalking()
    {
        int introSentenceLength = introTextManager.GetComponent<TextManager>().sentencesLength;
        int outroSentenceLength = outroTextManager.GetComponent<TextManager>().sentencesLength;

        // Intro Dialogue
        if (introSentenceLength == 2)
        {
            GameController.instance.CharacterTalking("leftIsTalking", true);
            GameController.instance.CharacterTalking("rightIsTalking", false);
        }
        else if (introSentenceLength == 1)
        {
            GameController.instance.CharacterTalking("leftIsTalking", false);
            GameController.instance.CharacterTalking("rightIsTalking", true);
        }
        
        // Outro Dialogue
        else if (outroSentenceLength == 3)
        {
            GameController.instance.CharacterTalking("leftIsTalking", false);
            GameController.instance.CharacterTalking("rightIsTalking", true);
        }
        else if (outroSentenceLength == 2)
        {
            GameController.instance.CharacterTalking("leftIsTalking", true);
            GameController.instance.CharacterTalking("rightIsTalking", false);
        }
        else if (outroSentenceLength == 1)
        {
            GameController.instance.CharacterTalking("leftIsTalking", false);
            GameController.instance.CharacterTalking("rightIsTalking", true);
        }
    }

    // Helper function: For a button to skip the intro tutorial dialogue
    public void IntroDialogueSkipButton()
    {
        GameController.instance.TutorialIntroDialogueDone();
    }

    void Tutorials()
    {
        // Run as long as the tutorial is not end
        if (!tutorialEnd)
        {
            // Tutorial part 1 finish
            if (isPhase1Set && _HandManager.player_hands_holdCards.Count == 0)
            {
                if (isPhase2Set == false)
                {
                    Phase_2_Setup();
                }
                else
                {
                    // Tutorial part 3 finish
                    Phase_3_Setup();
                }
            }
        }
        // Tutorial part 3 is over
        else
        {
            if (!_historyTutorialStarted)
            {
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    historyTutorial.SetActive(true);
                }, 7f));
                _historyTutorialStarted = true;
            }
            
            if (Scrollbareffect.instance.firstHistoryClick)
            {
                historyTutorial.SetActive(false);
            }
        }
    }

private bool outroTextStarted = false;
    void LevelManagement()
    {
        // Player wins and the tutorial is over
        if (BattleController.instance.enemy.Health_Current <= 0)
        {
            historyTutorial.SetActive(false);
            tutorialEnd = true;
            GameController.instance.tutorialLevelEnd = true;
            GameController.instance.DisableBattleController();
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                GameController.instance.DisableBattleMode();
                GameController.instance.RestartDialogue();
            }, 2f));
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                if (outroTextStarted == false)
                {
                    outroTextBox.SetActive(true);
                    outroTextManager.SetActive(true);
                    outroTextStarted = true;
                }
            }, 6f));
        }
        
        // Player loses
        if (BattleController.instance.player.Health_Current <= 0)
        {
            GameController.instance.DisableBattleMode();
            SceneManager.LoadScene("EndScene");
        }
    }

    void Phase_1_Setup()
    {
        _DeckSystem.deckToUse.Clear();
        BattleController.instance.startDrawingCards = false;
        foreach (Card_Basedata card in tutorialCards_P1)
        {
            _DeckSystem.deckToUse.Add(card);
        }

        GameController.instance.TutorialBattleSetup();
        isPhase1Set = true;

        _DeckSystem.DrawMultipleCardsThenStopDrawFeature(1);
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

        _DeckSystem.DrawMultipleCardsThenStopDrawFeature(2);
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
        }, 5f));
    }

    void Backupdata()
    {
        BattleController.instance.startingCardsAmount = Backup_StartDraw;
        BattleController.instance.startDrawingCards = true;
        _DeckSystem.enableDrawing = true;
    }

    private void OnDestroy()
    {
        //Backupdata();
    }

    public void NextButton()
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 1);
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
        }
        
        // Tutorial03
        if (tutorial03.activeSelf)
        {
            if (tutorial03_Pages[0].activeSelf)
            {
                tutorial03_Pages[0].SetActive(false);
                tutorial03_Pages[1].SetActive(true);
            }
            // For additional pages
            // else if (tutorial03_Pages[1].activeSelf)
            // {
            //     tutorial03_Pages[1].SetActive(false);
            //     tutorial03_Pages[2].SetActive(true);
            // }
        }
    }

    public void PrevButton()
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 1);
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
