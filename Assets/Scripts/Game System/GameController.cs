using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //flag will be turned on when setup function needed
    [HideInInspector] public bool setupFlag = false;


    [SerializeField] BattleController _script_BattleController;
    [SerializeField] HandManager _script_HandManager;
    [SerializeField] EffectDictionary _script_EffectDictionary;
    [SerializeField] PrioritySystem _script_PrioritySystem;
    [SerializeField] DeckSystem _script_DeckSystem;

    // Start is called before the first frame update
    void Start()
    {
        testPos = GameObject.Find("Enemy").GetComponent<Transform>();
    }


    public GameObject testEffect;
    public Transform testPos;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main Menu");
        }


        if (Input.GetKeyDown(KeyCode.T))
        {
            BattleSystemSetUp();
        }

        if (setupFlag)
        {
            BattleSystemSetUp();
            setupFlag = false;
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
