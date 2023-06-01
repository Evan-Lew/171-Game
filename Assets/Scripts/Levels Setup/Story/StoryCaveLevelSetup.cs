using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCaveLevelSetup : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject textManager;
    
    void Start()
    {
        GameController.instance.ChangeBackground("Cave_BG");
        GameController.instance.StartDialogue("Cave");
        
        // Time delay to activate the dialogue text box
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            //GameController.instance.CharacterTalking("Bai Suzhen", true);
            textBox.SetActive(true);
            textManager.SetActive(true);
        }, 2f));
    }
}