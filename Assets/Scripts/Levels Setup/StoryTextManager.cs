using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

public class StoryTextManager : MonoBehaviour
{
    public GameObject scrollObj;
    public TMP_Text dialogueText;
    private Queue<string> _sentences;
    public Dialogue dialogue;
    [SerializeField] GameObject mapButton;

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
    
    private void Start()
    {   
        _sentences = new Queue<string>();
        StartDialogue(dialogue);
    }
    
    private void StartDialogue(Dialogue moreDialogue)
    {
        _sentences.Clear(); 
        foreach (string sentence in moreDialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }
        DisplayNext();
    }

    public void DisplayNext()
    {
        Debug.Log(_sentences.Count);
        HighlightCharacterTalking();
        
        
        // Check if the sentence is typing
        if (_isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = _currSentence;
            _isTyping = false;
        }
        else if (_sentences.Count == 0){
            SceneManager.LoadScene("BattleLevel");
        }
        else
        {
            _isTyping = true;
            string sentence = _sentences.Dequeue();
            _currSentence = sentence;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));    
        }
    }
    private IEnumerator TypeSentence (string sentence)
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
        if (_sentences.Count == 2)
        {
            GameController.instance.CharacterTalking("leftIsTalking", true);
            GameController.instance.CharacterTalking("rightIsTalking", false);
        }
        else if (_sentences.Count == 1)
        {
            GameController.instance.CharacterTalking("leftIsTalking", false);
            GameController.instance.CharacterTalking("rightIsTalking", true);
        }
    }
}