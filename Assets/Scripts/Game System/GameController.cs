using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //flag will be turned on when setup function needed
    [HideInInspector] private bool setupFlag = false;


    [SerializeField] BattleController _script_BattleController;
    [SerializeField] HandManager _script_HandManager;
    [SerializeField] EffectDictionary _script_EffectDictionary;
    [SerializeField] PrioritySystem _script_PrioritySystem;
    [SerializeField] DeckSystem _script_DeckSystem;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main Menu");
        }


        if (Input.GetKeyDown(KeyCode.T))
        {
            SoundManager.PlaySound("bgm_Mountain_Of_Myths", 0.1f);
            setupFlag = true;
            if (setupFlag)
            {
                BattleSystemSetUp();
                setupFlag = false;
            }
        }





        if (Input.GetKeyDown(KeyCode.M))
        {


        }

    }

    //setup card system
    void BattleSystemSetUp()
    {
        //don't change order of this before you read all SetUp();
        _script_HandManager.SetUp();
        _script_DeckSystem.SetUp();
        _script_BattleController.SetUp();
        _script_EffectDictionary.SetUp();
    }



}
