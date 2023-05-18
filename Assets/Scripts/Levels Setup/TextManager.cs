using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    public GameObject scroll;
    //public Text nameText;   //for NPCs
    public TMP_Text dialogueText;
    public Queue<string> sentences;
    public Dialogue dialogue;

    [HideInInspector] public int sentencesLength;
    private bool tutorialLevelLoaded = false;
    
    void Start()
    {   
        sentences = new Queue<string>();
        StartDialogue(dialogue);
        IsTutorialLoaded();
    }

    // Helper function to see if the TutorialLevel scene is loaded
    private void IsTutorialLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // Check if the scene is loaded and is not an empty scene
            if (scene.isLoaded && !string.IsNullOrEmpty(scene.name))
            {
                if (scene.name == "TutorialLevel")
                {
                    tutorialLevelLoaded = true;
                }
            }
        }
        Debug.Log(tutorialLevelLoaded);
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
        int count = 0;
        
        // Disable the scroll after all sentences are read
        if (sentences.Count == count)
        {
            scroll.SetActive(false);
        }
        if (sentences.Count == 0){
            // Start the battle after the intro tutorial dialogue
            if (GameController.instance.tutorialIntroDialoguePlaying && tutorialLevelLoaded)
            {
                GameController.instance.TutorialIntroDialogueDone();    
            }

            // End the battle after the end tutorial dialogue
            if (GameController.instance.tutorialOutroDialoguePlaying && GameController.instance.tutorialLevelEnd && tutorialLevelLoaded)
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
