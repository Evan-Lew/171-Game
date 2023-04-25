using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextManager : MonoBehaviour
{
 //public Text nameText;   //for NPCs
   public TMP_Text dialogueText;
   public Queue<string> sentences;
   public Dialogue dialogue;
    void Start()
    {   
        sentences = new Queue<string>();
        StartDialogue(dialogue);
    }
    public void StartDialogue (Dialogue dialogue)
    {
        sentences.Clear(); 
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNext();
    }
    public void DisplayNext()
    {
        if (sentences.Count == 0){
            //EndDialogue(); //make another function, im lazy
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
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
