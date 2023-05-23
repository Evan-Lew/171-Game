using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryTextManager : MonoBehaviour
{
    // Variables
    public TMP_Text dialogueText;
    [SerializeField] GameObject mapButton;
    [SerializeField] List<GameObject> textManagersList;
    [SerializeField] List<GameObject> textBackgroundsList;
    [SerializeField] List<Animator> animatorStoryTextFade;

    private Queue<string> _sentences;
    public Dialogue dialogue;

    void Awake()
    {
        _sentences = new Queue<string>();
        
        Debug.Log("Story scenes left: " + GameController.instance.storyScenesLeft);
        Debug.Log("Scenes played: " + GameController.instance.scenesPlayed);
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            animatorStoryTextFade[GameController.instance.scenesPlayed].SetTrigger("FadeIn");
        }, 1.5f));
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
        if (_sentences.Count == 0)
        {
            animatorStoryTextFade[GameController.instance.scenesPlayed].SetTrigger("FadeOut");
            GameController.instance.storyScenesLeft -= 1;
            GameController.instance.scenesPlayed += 1;
            
            // Intro is over
            if (GameController.instance.storyScenesLeft == 0){
                //mapButton.SetActive(true);
                GameController.instance.FadeOut();
            }
            else
            {
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
            string sentence = _sentences.Dequeue();
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