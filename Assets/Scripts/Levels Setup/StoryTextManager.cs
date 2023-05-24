using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryTextManager : MonoBehaviour
{
    public GameObject scrollObj;
    public TMP_Text dialogueText;
    public Queue<string> sentences;
    public Dialogue dialogue;
    [SerializeField] GameObject mapButton;

    // Variables in order to skip the text typing
    private bool _isTyping = false;
    private string _currSentence;

    void Start()
    {   
        sentences = new Queue<string>();
        StartDialogue(dialogue);
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
        else if (sentences.Count == 0){
            SceneManager.LoadScene("BattleLevel");
        }
        else
        {
            _isTyping = true;
            string sentence = sentences.Dequeue();
            _currSentence = sentence;
            StopAllCoroutines();
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