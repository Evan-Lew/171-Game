using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    //flag will be turned on when setup function needed
    [HideInInspector] private bool setupFlag = false;
    [SerializeField] CameraUtil _script_CameraUtil;
    [SerializeField] BattleController _script_BattleController;
    [SerializeField] HandManager _script_HandManager;
    [SerializeField] EffectDictionary _script_EffectDictionary;
    [SerializeField] PrioritySystem _script_PrioritySystem;
    [SerializeField] DeckSystem _script_DeckSystem;
    [SerializeField] DeckEditSystem _script_DeckEditSystem;


    [SerializeField] GameObject characters;
    [SerializeField] GameObject player, enmey;
    [SerializeField] List<Character_Basedata> EnemyList = new();
    Character_Basedata newEnemy;

    [Header("Don't change order!")]
    [SerializeField] List<GameObject> CamerasObj;

    //set position -- character and enemy
    [SerializeField] List<GameObject> CharacterSpawningPoint_List = new();
    [SerializeField] List<GameObject> CameraSpawningPoint_List = new();
    public GameObject TargetCharacterPos;
    public GameObject TargetCameraPos;


    //flag for changing level
    [HideInInspector]public bool isDeckELevel = true;
    [HideInInspector] public bool isStartLevel = false;


    private void Awake()
    {
        //SceneManager.LoadScene("Main Menu");
        characters.SetActive(false);
        //SetSpawningPoint(TargetCharacterPos.transform, TargetCameraPos.transform);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (isDeckELevel)
        //{
        //    isDeckELevel = false;
        //    StartTheCamp();
        //}
        //else if (isStartLevel)
        //{
        //    isStartLevel = false;
        //}
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartTheCamp();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            SoundManager.PlaySound("bgm_Mountain_Of_Myths", 0.1f);
            setupFlag = true;
            if (setupFlag)
            {
                StartTheBattle(GetEnemy("Ink Golem"), true);
                setupFlag = false;
            }
        }
        BattleConditionCheck();
    }

    //===========================================================
    //                  GameController API
    //===========================================================
    /*  Function that will setup spawning point for characters group and Camera
     *  Parameters: Argument1:  Target Character Group game object's transform
     *              Argument2:  Target Environment Camera's transform                         
     */
    bool checkEnable = true;
    void BattleConditionCheck()
    {
        if(checkEnable)
        {
            if (enmey.GetComponent<Character>().Health_Current <= 0 || player.GetComponent<Character>().Health_Current <= 0)
            {
                _script_DeckSystem.deckToUse.Clear();
                _script_DeckSystem.Clear();
                _script_HandManager.Clear();
                player.GetComponent<Character>().Health_Current = player.GetComponent<Character>().Health_Total;
                checkEnable = false;
                _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
                _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
                SceneManager.LoadScene("Main Menu");
            }
        }
    }
    public void SetSpawningPoint(Transform characterTransform, Transform environmentCameraTransform)
    {
        SetCharacterPos(characterTransform);
        SetCameraPos(environmentCameraTransform);
    }
    
    /*  Function that will start the battle for testing, assigned deck is required
    *  Parameters:  Argument1:  The next enemy you want to setup 
    *               Argument2 (override):  true/false, doesn't matter
    *                                     this version will allow to start battle without deck = 10 cards
    */
    void StartTheBattle(Character_Basedata enemy, bool overrideVer)
    {
        BattleSystemSetUp(enemy);
    }

    //public Character_Basedata playTest2;
    //public void PlayTestStartBattle()
    //{
    //     StartTheBattle(playTest2);
    //}

    void StartTheBattle(Character_Basedata enemy)
    {
        if (_script_DeckSystem.deckToUse.Count == 10)
        {
            checkEnable = true;
            BattleSystemSetUp(enemy);
        }
    }

    /*  Funtion that will start camp view
    *  Parameters:  void
    */
    void StartTheCamp()
    {
        CampSystemSetUp();
    }
    //                GameController API End
    //===========================================================




    //Use API Above
    //==========================================================================================================================
    //Ignore all function below




    //===========================================================
    //                  Helper Functions
    //===========================================================

    /* Function that will get Enemy From the Enemy List by Name
     * Parameters: Argument1:  Target Enemy Name
     * Return:     Character_Basedata an Enemy basedata or error if enemy is not found or not unique
     */
    private Character_Basedata GetEnemy(string name)
    {
        Character_Basedata result;
        result = EnemyList.Where(enemyBasedata => enemyBasedata.name == name).SingleOrDefault();
        if (result != null)
        {
            return result;
        }
        else
        {
            Debug.Log("Error: GetEnemy() in GameController. No such enemy is found or enmey is not unique");
            return result;
        }
    }

    //StartTheBattle(Character_Basedata enemy, bool overrideVer) or StartTheBattle(Character_Basedata enemy)
    /*  Function that will setup battle system
     *  
     *  Parameters: Argument1:  Target Character Group game object's transform
     *              Argument2:  Target Environment Camera's transform                         
     */
    void BattleSystemSetUp(Character_Basedata enemy)
    {
        //don't change order of call
        SetEnemy(enemy);
        characters.SetActive(true);
        //implement the character reassignment here
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), true);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_HandManager.SetUp();
        _script_DeckSystem.SetUp();
        _script_BattleController.SetUp();
        _script_EffectDictionary.SetUp();
    }

    //StartTheCamp()
    /*  Function that will setup battle system
    *  
    *   Parameters: Argument1:  Target Character Group game object's transform
    *               Argument2:  Target Environment Camera's transform                         
    */
    void CampSystemSetUp()
    {
     
        characters.SetActive(false);
        _script_BattleController.Clear();
        _script_DeckSystem.Clear();
        _script_HandManager.Clear();
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), true);
    }


    //BattleSystemSetUp(Character_Basedata enemy)
    void SetEnemy(Character_Basedata newEnemy)
    {
        Character enemyCharacter = enmey.GetComponent<Character>();
        enemyCharacter.CharacterData = newEnemy;
        enemyCharacter.SetUp();
    }



    //SetSpawningPoint()
    private void SetCharacterPos(Transform targetTrans)
    {
        characters.transform.position = targetTrans.position;
        characters.transform.rotation = targetTrans.rotation;
    }

    //SetSpawningPoint()
    private void SetCameraPos(Transform targetTrans)
    {
        CamerasObj[0].transform.position = targetTrans.position;
        CamerasObj[0].transform.rotation = targetTrans.rotation;
    }

    //                  Helper Function End
    //===========================================================




}
