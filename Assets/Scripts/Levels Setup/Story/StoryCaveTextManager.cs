using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;
public class StoryCaveTextManager : MonoBehaviour
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
            GameController.instance.EndDialogue("Cave");
            SceneManager.LoadScene("BattleMap");
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
                GameController.instance.RatioOutandBrightenScreen();
                GameController.instance.CharacterTalking("Bai Suzhen", false);

                // Delay for the fade out
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    GameController.instance.EndDialogue("Cave");
                    SceneManager.LoadScene("BattleMap");
                }, 7f));
            }
            else
            {
                _isTyping = true;
                string sentence = _sentences.Dequeue();
                _currSentence = sentence;
                StopAllCoroutines();
                StartCoroutine(TypeSentence(sentence));
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    {
                        SoundManager.PlaySound("sfx_Wood_Fish", 0.1f); 
                    } 
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
        if (_sentences.Count == 19)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
        }
        else if (_sentences.Count == 18)
        {
            GameController.instance.CharacterTalking("Xuan Wu", true);
        }
        else if (_sentences.Count == 17)
        {
            GameController.instance.CharacterTalking("Bi Zi", true);
            GameController.instance.CharacterTalking("Xuan Wu", false);
        }
        else if (_sentences.Count == 16)
        {
            GameController.instance.CharacterTalking("Bi Zi", false);
            GameController.instance.CharacterTalking("Xuan Wu", true);
        }
        else if (_sentences.Count == 15)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Xuan Wu", false);
        }
        else if (_sentences.Count == 14)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
            GameController.instance.CharacterTalking("Xuan Wu", true);
        }
        else if (_sentences.Count == 13)
        {
            
        }
        else if (_sentences.Count == 12)
        {
            
        }
        else if (_sentences.Count == 11)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Xuan Wu", false);
        }
        else if (_sentences.Count == 10)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
            GameController.instance.CharacterTalking("Xuan Wu", true);
        }
        else if (_sentences.Count == 9)
        {
            
        }
        else if (_sentences.Count == 8)
        {
            
        }
        else if (_sentences.Count == 7)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Xuan Wu", false);
        }
        else if (_sentences.Count == 6)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
            GameController.instance.CharacterTalking("Xuan Wu", true);
        }
        else if (_sentences.Count == 5)
        {
            GameController.instance.BiZiLookAroundDialogue();
            GameController.instance.CharacterTalking("Bi Zi", true);
            GameController.instance.CharacterTalking("Xuan Wu", false);
        }
        else if (_sentences.Count == 4)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Bi Zi", false);
        }
        else if (_sentences.Count == 3)
        {
            GameController.instance.CharacterTalking("Bai Suzhen", false);
            GameController.instance.CharacterTalking("Xuan Wu", true);
        }
        else if (_sentences.Count == 2)
        {
            GameController.instance.CharacterTalking("Xuan Wu", false);
            GameController.instance.XuanWuDisappearLeftDialogue();
        }
        else if (_sentences.Count == 1)
        {
            GameController.instance.BiZiBaiSuzhenDisappearLeftDialogue();
            GameController.instance.CharacterTalking("Bai Suzhen", true);
            GameController.instance.CharacterTalking("Xuan Wu", false);
        }
    }
}
