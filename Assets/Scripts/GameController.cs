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


    private void Awake()
    {
        BattleSystemSetUp();
    }


    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("Main Menu");
        }

        if (setupFlag)
        {
            BattleSystemSetUp();
            setupFlag = false;
        }
    }

    //setup card system
    void BattleSystemSetUp()
    {
        _script_BattleController.SetUp();
        _script_HandManager.SetUp();
        _script_EffectDictionary.SetUp();
    }



}
