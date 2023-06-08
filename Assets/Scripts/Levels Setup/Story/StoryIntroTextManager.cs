using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryIntroTextManager : MonoBehaviour
{
    // Variables
    public TMP_Text dialogueText;
    [SerializeField] List<GameObject> textManagersList;
    [SerializeField] List<GameObject> textBackgroundsList;
    [SerializeField] List<Animator> animatorStoryTextFade;
    [SerializeField] GameObject nextButton;
    
    // End Text
    [SerializeField] GameObject endText;
    [SerializeField] Animator animatorEndText;

    private Queue<string> _sentences;
    public Dialogue dialogue;
    
    // Variables in order to skip the text typing
    private bool _isTyping = false;
    private string _currSentence;
    private bool _transitioning = false;

    // Developer Tool

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene("TutorialLevel");
        }

        // Boolean to ensure that the text box is active
        if (_transitioning)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNext();
            }
            if (Input.GetKeyDown(KeyCode.Tab)) 
            {
                for (int i = 0; i < GameController.instance.storyBackgroundsList.Count; i++)
                {
                    GameController.instance.storyBackgroundsList[i].SetActive(false);
                }
                SceneManager.LoadScene("StoryVillageLevel");
            }
        }
    }

    void Awake()
    {
        _sentences = new Queue<string>();
        
        //Debug.Log("Story scenes left: " + GameController.instance.storyScenesLeft);
        //Debug.Log("Scenes played: " + GameController.instance.scenesPlayed);
        if (GameController.instance.storyScenesLeft > 0)
        {
            // Delay for the text box fade in
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                animatorStoryTextFade[GameController.instance.scenesPlayed].SetTrigger("FadeIn");
                _transitioning = true;
            }, 1.5f));    
        }
        StartDialogue(dialogue);
    }

    public void StartDialogue(Dialogue moreDialogue)
    {
        _sentences.Clear(); 
        foreach (string sentence in moreDialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }
        // Delay to account for the fade in 
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            DisplayNext();
        }, 2f));
    }

    public void DisplayNext()
    {
        // Check if the sentence is typing
        if (_isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = _currSentence;
            _isTyping = false;
        }
        else if (_sentences.Count == 0)
        {
            nextButton.SetActive(false);
            _transitioning = false;
            animatorStoryTextFade[GameController.instance.scenesPlayed].SetTrigger("FadeOut");
            GameController.instance.storyScenesLeft -= 1;
            GameController.instance.scenesPlayed += 1;

            // Intro is over
            if (GameController.instance.storyScenesLeft == 0)
            {
                textBackgroundsList[3].SetActive(true);
                //mapButton.SetActive(true);
                GameController.instance.FadeOut();
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    animatorEndText.SetTrigger("FadeIn");
                    textManagersList[4].SetActive(true);
                }, 4f));   
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    animatorEndText.SetTrigger("FadeOut");
                    SceneManager.LoadScene("TutorialLevel");
                }, 10f));  
            }
            else
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    SoundManager.PlaySound("sfx_Wood_Fish", 0.1f); 
                }
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    // Remove the current textManager and activate the next textManager
                    textManagersList[GameController.instance.scenesPlayed - 1].SetActive(false);
                    textManagersList[GameController.instance.scenesPlayed].SetActive(true);
                    // Remove the current text and activate the next text
                    textBackgroundsList[GameController.instance.scenesPlayed - 1].SetActive(false);
                    textBackgroundsList[GameController.instance.scenesPlayed].SetActive(true);
                    // Change the background
                    GameController.instance.ChangeStoryBackground(GameController.instance.scenesPlayed - 1);
                }, 1f));       
            }
        }
        else
        {
            _isTyping = true;
            string sentence = _sentences.Dequeue();
            _currSentence = sentence;
            StopAllCoroutines(); 
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    SoundManager.PlaySound("sfx_Wood_Fish", 0.1f); 
                }
            StartCoroutine(TypeSentence(sentence)); 
        }
    }
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        _isTyping = false;
    }
}
