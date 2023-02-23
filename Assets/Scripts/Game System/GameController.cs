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
    [SerializeField] List<GameObject> CamerasObj;
    [SerializeField] GameObject characters;



    private void Awake()
    {
        //note the 1 means the 1 index of building list
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        characters.SetActive(false);
    }

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
                StartTheBattle(true);
                setupFlag = false;
            }
        }

    }
    //override version
    void StartTheBattle(bool overrideVer)
    {
        //SceneManager.LoadScene("Stage01", LoadSceneMode.Additive);
        BattleSystemSetUp();
    }

    public void StartTheBattle()
    {
        if(_script_DeckSystem.deckToUse.Count == 10)
        {
            //SceneManager.LoadScene("Stage01", LoadSceneMode.Additive);
            BattleSystemSetUp();
        }

    }

    //setup card system
    void BattleSystemSetUp()
    {
        //don't change order of this before you read all SetUp();
        characters.SetActive(true);
        _script_CameraUtil.SetCameraActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), true);
        _script_CameraUtil.SetCameraActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_HandManager.SetUp();
        _script_DeckSystem.SetUp();
        _script_BattleController.SetUp();
        _script_EffectDictionary.SetUp();
        
    }


    void CampSystemSetUp()
    {

    }


}
