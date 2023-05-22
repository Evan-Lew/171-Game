using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryTextManager : MonoBehaviour
{
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
        StartDialogue(dialogue);

        //animatorStoryTextFade[0].SetTrigger("FadeIn");
        Debug.Log(GameController.instance.storyScenesLeft);
  
        // if (GameController.instance.storyScenesLeft >= 3)
        // {
        //     StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        //     {
        //         animatorStoryTextFade[0].SetTrigger("FadeIn");
        //     }, 1.5f));    
        // }
        // else if (GameController.instance.storyScenesLeft == 2)
        // {
        //     StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        //     {
        //         animatorStoryTextFade[1].SetTrigger("FadeIn");
        //     }, 1.5f));    
        // }
        
        if (GameController.instance.storyScenesLeft == 4)
        {
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                animatorStoryTextFade[0].SetTrigger("FadeIn");
            }, 1.5f));    
        }
        else if (GameController.instance.storyScenesLeft == 3)
        {
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                animatorStoryTextFade[1].SetTrigger("FadeIn");
            }, 1.5f));    
        }
        else if (GameController.instance.storyScenesLeft == 2)
        {
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                animatorStoryTextFade[2].SetTrigger("FadeIn");
            }, 1.5f));    
        }
        else if (GameController.instance.storyScenesLeft == 1)
        {
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                animatorStoryTextFade[3].SetTrigger("FadeIn");
            }, 1.5f));    
        }
    }

    public void StartDialogue(Dialogue moreDialogue)
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
        if (_sentences.Count == 0 && GameController.instance.storyScenesLeft == 0){
            mapButton.SetActive(true);
        }
        else if (_sentences.Count == 0)
        {
            animatorStoryTextFade[0].SetTrigger("FadeOut");
            GameController.instance.storyScenesLeft -= 1;

            if (GameController.instance.storyScenesLeft == 3)
            {
                TransitionFromImg1To2();    
            }
            else if (GameController.instance.storyScenesLeft == 2)
            {
                TransitionFromImg2To3();
            }
            else if (GameController.instance.storyScenesLeft == 1)
            {
                TransitionFromImg3To4();
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

    private void TransitionFromImg1To2()
    {
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            textManagersList[0].SetActive(false);
            textManagersList[1].SetActive(true);
            textBackgroundsList[0].SetActive(false);
            textBackgroundsList[1].SetActive(true);
            GameController.instance.ChangeStoryBackground(0);
        }, 1f));
    }
    
    private void TransitionFromImg2To3()
    {
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            textManagersList[1].SetActive(false);
            textManagersList[2].SetActive(true);
            textBackgroundsList[1].SetActive(false);
            textBackgroundsList[2].SetActive(true);
            GameController.instance.ChangeStoryBackground(1);
        }, 1f));
    }
    
    private void TransitionFromImg3To4()
    {
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            textManagersList[2].SetActive(false);
            textManagersList[3].SetActive(true);
            textBackgroundsList[2].SetActive(false);
            textBackgroundsList[3].SetActive(true);
            GameController.instance.ChangeStoryBackground(2);
        }, 1f));
    }
}
