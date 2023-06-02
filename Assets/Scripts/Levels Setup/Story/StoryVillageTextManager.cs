using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

public class StoryVillageTextManager : MonoBehaviour
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
                GameController.instance.FaHaiDisappearLeftDialogue();

                // Delay for the fade out
                StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
                {
                    GameController.instance.EndDialogue("Village");
                    SceneManager.LoadScene("StoryForestLevel");
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
        if (_sentences.Count == 12)
        {
            GameController.instance.CharacterTalking("Xu Xian", true);
            GameController.instance.CharacterTalking("Fa Hai", false);
        }
        else if (_sentences.Count == 11)
        {
            GameController.instance.FlashOfLightAppearDialogue();
            
            GameController.instance.CharacterTalking("Xu Xian", false);
            GameController.instance.CharacterTalking("Fa Hai", false);
        }
        else if (_sentences.Count == 10)
        {
            GameController.instance.FlashOfLightDisappearDialogue();
            GameController.instance.TurnToSnakeDialogue();
        }
        else if (_sentences.Count == 9)
        {
            GameController.instance.CharacterTalking("Xu Xian", true);
            GameController.instance.CharacterTalking("Fa Hai", false);
        }
        else if (_sentences.Count == 8)
        {
            GameController.instance.CharacterTalking("Xu Xian", false);
            GameController.instance.CharacterTalking("Fa Hai", true);
        }
        else if (_sentences.Count == 7)
        {
            GameController.instance.CharacterTalking("Xu Xian", true);
            GameController.instance.CharacterTalking("Fa Hai", false);
        }
        else if (_sentences.Count == 6)
        {
            GameController.instance.CharacterTalking("Xu Xian", false);
            GameController.instance.CharacterTalking("Fa Hai", false);
        }
        else if (_sentences.Count == 5)
        {
            GameController.instance.XuXianMoveRightDialogue();
        }
        else if (_sentences.Count == 4)
        {
            GameController.instance.FaHaiAttackDialogue();
            GameController.instance.HurtBackgroundDialogue();
            GameController.instance.XuXianHurtDialogue();
        }
        else if (_sentences.Count == 3)
        {
            GameController.instance.BaiSuzhenRunDialogue();
        }
        else if (_sentences.Count == 2)
        {
            GameController.instance.CharacterTalking("Xu Xian", false);
            GameController.instance.CharacterTalking("Fa Hai", true);
        }
        else if (_sentences.Count == 0)
        {
            GameController.instance.CharacterTalking("Fa Hai", false);
        }

    }
}