using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextManager : MonoBehaviour
{
    public TutorialSetup tutorialSetup;
    //public Text nameText;   //for NPCs
    public TMP_Text dialogueText;
    public Queue<string> sentences;
    public Dialogue dialogue;

    public int sentencesLength;
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
        sentencesLength = sentences.Count;
        if (sentences.Count == 0){
            if (GameController.instance.tutorialIntroDialoguePlaying)
            {
                GameController.instance.TutorialIntroDialogueDone();    
            }

            if (GameController.instance.tutorialOutroDialoguePlaying && GameController.instance.tutorialLevelEnd)
            {
                GameController.instance.TutorialOutroDialogueDone(); 
            }
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
