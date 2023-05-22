using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryTextManager : MonoBehaviour
{
    public GameObject textGroupGameObject;
    public TMP_Text dialogueText;
    [SerializeField] GameObject mapButton;
    [SerializeField] List<GameObject> textManagersList;
    private int _scenesLeft = 1;
    
    private Queue<string> _sentences;
    public Dialogue dialogue;

    void Start()
    {   
        GameController.instance.FadeIn();
        _sentences = new Queue<string>();
        StartDialogue(dialogue);
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
        if (_sentences.Count == 0 && _scenesLeft == 0){
            mapButton.SetActive(true);
        }
        else if (_sentences.Count == 0)
        {
            textGroupGameObject.SetActive(false);
            
            _scenesLeft -= 1;
            StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
            {
                textManagersList[0].SetActive(false);
                textManagersList[1].SetActive(true);
                textGroupGameObject.SetActive(true);
            }, 3f));
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
