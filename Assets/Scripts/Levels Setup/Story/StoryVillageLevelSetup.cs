using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryVillageLevelSetup : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject textManager;
    
    void Start()
    {
        GameController.instance.ChangeBackground("Village_BG");
        GameController.instance.StartDialogue("Village");
        SoundManager.PlaySound("bgm_Tea", 0.3f);
        // Time delay to activate the dialogue text box
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            GameController.instance.CharacterTalking("Xu Xian", true);
            textBox.SetActive(true);
            textManager.SetActive(true);
        }, 4f));
    }
}
