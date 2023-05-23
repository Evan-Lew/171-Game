using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryLevelSetup : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject textManager;
    
    void Start()
    {
        GameController.instance.ChangeBackground("Village_BG");
        
        
        GameController.instance.StartDialogue();
        // Time delay to activate the dialogue text box
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            GameController.instance.CharacterTalking("rightIsTalking", true);
            textBox.SetActive(true);
            textManager.SetActive(true);
        }, 4f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
