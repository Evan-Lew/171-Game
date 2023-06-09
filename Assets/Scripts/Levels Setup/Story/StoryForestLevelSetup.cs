using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryForestLevelSetup : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject textManager;
    
    void Start()
    {
        GameController.instance.ChangeBackground("Forest_BG");
        GameController.instance.StartDialogue("Forest");
        //SoundManager.PlaySound("bgm_Tea", 0.15f);
        // Time delay to activate the dialogue text box
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            //GameController.instance.CharacterTalking("Bai Suzhen", true);
            textBox.SetActive(true);
            textManager.SetActive(true);
        }, 4f));
    }
}
