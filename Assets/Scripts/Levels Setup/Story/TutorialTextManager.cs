using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialTextManager : MonoBehaviour
{
    public GameObject scroll;
    //public Text nameText;   //for NPCs
    public TMP_Text dialogueText;
    public Queue<string> sentences;
    public Dialogue dialogue;
    [SerializeField] GameObject mapButton;
    [SerializeField] bool isIntro;

    [HideInInspector] public int numOfSentences;
    private bool _tutorialLevelLoaded = false;
    private bool _storyIntroLoaded = false;
    
    // Variables in order to skip the text typing
    private bool _isTyping = false;
    private string _currSentence;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNext();
        }
    }

    void Start()
    {   
        sentences = new Queue<string>();
        StartDialogue(dialogue);
        IsSceneLoaded();
    }

    // Helper function to see if the TutorialLevel scene is loaded
    private void IsSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // Check if the scene is loaded and is not an empty scene
            if (scene.isLoaded && !string.IsNullOrEmpty(scene.name))
            {
                if (scene.name == "TutorialLevel")
                {
                    _tutorialLevelLoaded = true;
                }
                else if (scene.name == "StoryIntro")
                {
                    _storyIntroLoaded = true;
                }
            }
        }
    }
    
    public void StartDialogue(Dialogue moreDialogue)
    {
        sentences.Clear(); 
        foreach (string sentence in moreDialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNext();
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
        else
        {
            HighlightCharacterTalking();
            numOfSentences = sentences.Count;
            if (sentences.Count == 0){
                scroll.SetActive(false);
                // Start the battle after the intro tutorial dialogue
                if (GameController.instance.tutorialIntroDialoguePlaying && _tutorialLevelLoaded)
                {
                    GameController.instance.TutorialIntroDialogueDone();    
                }

                // End the battle after the end tutorial dialogue
                if (GameController.instance.tutorialOutroDialoguePlaying && GameController.instance.tutorialLevelEnd && _tutorialLevelLoaded)
                {
                    GameController.instance.TutorialOutroDialogueDone(); 
                }
            
                if (_storyIntroLoaded)
                {
                    mapButton.SetActive(true);
                }
            }
            else
            {
                _isTyping = true;
                string sentence = sentences.Dequeue();
                _currSentence = sentence;
                StopAllCoroutines();
                StartCoroutine(TypeSentence(sentence));  
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    {
                        SoundManager.PlaySound("sfx_Wood_Fish", 0.025f); 
                    }  
            }
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

    public void HighlightCharacterTalking()
    {
        if (isIntro)
        {
            if (sentences.Count == 4)
            {
                SoundManager.PlaySound("sfx_Calligraphy", 0.05f);
                GameController.instance.CharacterTalking("Xu Xian", true);
                GameController.instance.CharacterTalking("Fa Hai", false);
            }
            else if (sentences.Count == 3)
            {
                SoundManager.PlaySound("sfx_Calligraphy", 0.05f);
                GameController.instance.CharacterTalking("Xu Xian", false);
                GameController.instance.CharacterTalking("Fa Hai", true);
            }
            else if (sentences.Count == 2)
            {
                SoundManager.PlaySound("sfx_Calligraphy", 0.05f);
                GameController.instance.CharacterTalking("Xu Xian", false);
                GameController.instance.CharacterTalking("Fa Hai", true);
            }
            else if (sentences.Count == 1)
            {
                SoundManager.PlaySound("sfx_Calligraphy", 0.05f);
                GameController.instance.CharacterTalking("Xu Xian", false);
                GameController.instance.CharacterTalking("Fa Hai", true);
            }    
        }
        else
        {
            if (sentences.Count == 2)
            {
                SoundManager.PlaySound("sfx_Calligraphy", 0.05f);
                GameController.instance.CharacterTalking("Xu Xian", true);
                GameController.instance.CharacterTalking("Fa Hai", false);
            }
            else if (sentences.Count == 1)
            {
                SoundManager.PlaySound("sfx_Calligraphy", 0.05f);
                GameController.instance.CharacterTalking("Xu Xian", false);
                GameController.instance.CharacterTalking("Fa Hai", true);
            }
        }
    }
}
