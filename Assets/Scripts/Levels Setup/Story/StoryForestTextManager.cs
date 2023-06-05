using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;
public class StoryForestTextManager : MonoBehaviour
{
[SerializeField] GameObject textBox;
    public TMP_Text dialogueText;
    private Queue<string> _sentences;
    public Dialogue dialogue;

    // Variables in order to skip the text typing
    private bool _isTyping = false;
    private string _currSentence;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNext();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            // Need this to not make Bai Suzhen jump to the left
            GameController.instance.BaiSuzhenOffScreenLeft();
            GameController.instance.EndDialogue("Forest");
            SceneManager.LoadScene("StoryCaveLevel");
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
        //Debug.Log(_sentences.Count);
        
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
            if (_sentences.Count == 0)
            {
                textBox.SetActive(false);
                GameController.instance.FadeOut();

                // Delay for the fade out
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    GameController.instance.EndDialogue("Forest");
                    SceneManager.LoadScene("StoryCaveLevel");
                }, 7f));
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
        if (_sentences.Count == 10)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
        }
        else if (_sentences.Count == 9)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
        }
        else if (_sentences.Count == 8)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
        }
        else if (_sentences.Count == 7)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
        }
        else if (_sentences.Count == 6)
        {
            GameController.instance.BiZiAppearRightDialogue();
            GameController.instance.CharacterTalking("Bai Suzhen", false);
        }
        else if (_sentences.Count == 5)
        {
            GameController.instance.CharacterTalking("Bi Zi", true);
        }
        else if (_sentences.Count == 4)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Bi Zi", false);
        }
        else if (_sentences.Count == 3)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
            GameController.instance.CharacterTalking("Bi Zi", true);
        }
        else if (_sentences.Count == 2)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Bi Zi", false);
        }
        else if (_sentences.Count == 1)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
            GameController.instance.BiZiBaiSuzhenDisappearRightDialogue();
        }
    }
}