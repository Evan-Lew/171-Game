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
    [Header("Don't change order!")]
    [SerializeField] List<GameObject> CamerasObj;
    [SerializeField] GameObject characters;

    //set position -- character and enemy
    [SerializeField] List<GameObject> CharacterSpawningPoint_List = new();
    [SerializeField] List<GameObject> CameraSpawningPoint_List = new();
    public GameObject TargetCharacterPos;
    public GameObject TargetCameraPos;

    private void SetCharacterPos(Transform targetTrans)
    {
        characters.transform.position = targetTrans.position;
        characters.transform.rotation = targetTrans.rotation;
    }

    private void SetCameraPos(Transform targetTrans)
    {
        Debug.Log(" I am in set camera pos " + CamerasObj[0].transform.position);
        CamerasObj[0].transform.position = targetTrans.position;
        CamerasObj[0].transform.rotation = targetTrans.rotation;
        Debug.Log(targetTrans.position + " move " + CamerasObj[0].transform.position);
    }

    public void SetSpawningPoint(Transform characterTrans, Transform cameraTrans)
    {
        SetCharacterPos(characterTrans);
        SetCameraPos(cameraTrans);
    }

    private void Awake()
    {
        //note the 1 means the 1 index of building list
        //SceneManager.LoadScene(1, LoadSceneMode.Additive);
        characters.SetActive(false);
        //for testing(Holy shit!)
        SetSpawningPoint(TargetCharacterPos.transform, TargetCameraPos.transform);
        Debug.Log(" I am in set camera pos " + CamerasObj[0].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //SceneManager.LoadScene("Main Menu");
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
        BattleSystemSetUp();
    }

    public void StartTheBattle()
    {
        if(_script_DeckSystem.deckToUse.Count == 10)
        {
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
