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
        if (sentences.Count == 0){
            SceneManager.LoadScene("BattleLevel");
        }
        else
        {
            string sentence = sentences.Dequeue();
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
    }
}