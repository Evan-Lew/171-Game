using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    // Flag will be turned on when setup function needed
    
    /* --Legacy: Not Used--
    [HideInInspector] private bool setupFlag = false;
    */
    [SerializeField] CameraUtil _script_CameraUtil;
    [SerializeField] BattleController _script_BattleController;
    [SerializeField] HandManager _script_HandManager;
    [SerializeField] EffectDictionary _script_EffectDictionary;
    [SerializeField] PrioritySystem _script_PrioritySystem;
    [SerializeField] DeckSystem _script_DeckSystem;
    [SerializeField] DeckEditSystem _script_DeckEditSystem;
    
    [SerializeField] GameObject characters;
    [SerializeField] GameObject player, enemy;
    
    [FormerlySerializedAs("animatorSceneFade")]
    [Header("Animator Controllers")]
    [SerializeField] Animator animatorFadeScene;
    [SerializeField] private Animator animatorAspectRatioSwitch, animatorDarkenBackground, animatorXuXianDialogue, animatorFaHaiDialogue;

    [Header("Characters Talking")] 
    [SerializeField] GameObject leftCharacter;
    [SerializeField] GameObject rightCharacter;
    private SpriteRenderer _leftCharacterSprite;
    private SpriteRenderer _rightCharacterSprite;
    
    public List<Character_Basedata> CharactersList = new();
    Character_Basedata newEnemy;

    [Header("Don't change order!")]
    [SerializeField] List<GameObject> CamerasObj;
    
    // --Not used anymore--
    // Set position -- character and enemy
    // [SerializeField] List<GameObject> CharacterSpawningPoint_List = new();
    // [SerializeField] List<GameObject> CameraSpawningPoint_List = new();
    // public GameObject TargetCharacterPos;
    // public GameObject TargetCameraPos;
    
    // Flag for changing level
    [HideInInspector] public bool isDeckELevel = true;
    [HideInInspector] public bool isStartLevel = false;
    
    public static GameController instance;
    [HideInInspector] public bool enableMouseEffectOnCard = true;
    
    // New
    public bool tutorialIntroDialoguePlaying = true;
    
    private void Awake()
    {
        instance = this;
        characters.SetActive(false);
        // SetSpawningPoint(TargetCharacterPos.transform, TargetCameraPos.transform);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        
        // For dialogue
        _leftCharacterSprite = leftCharacter.GetComponent<SpriteRenderer>();
        _rightCharacterSprite = rightCharacter.GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        //BattleConditionCheck();
        
        //if (isDeckELevel)
        //{
        //    isDeckELevel = false;
        //    StartTheCamp();
        //}
        //else if (isStartLevel)
        //{
        //    isStartLevel = false;
        //}
        
        /* --Legacy: Not used--
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
                StartTheBattle(GetCharacter("Ink Golem"), true);
                setupFlag = false;
            }
        }
        */
    }
    
    public void StartDialogue()
    {
        animatorFadeScene.SetTrigger("FadeIn");
        animatorAspectRatioSwitch.SetTrigger("StartWithRatio");
        animatorDarkenBackground.SetTrigger("StartDark");
        leftCharacter.SetActive(true);
        rightCharacter.SetActive(true);
    }

    public void RestartDialogue()
    {
        animatorAspectRatioSwitch.SetTrigger("In");
        animatorDarkenBackground.SetTrigger("Dark");
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            animatorXuXianDialogue.SetTrigger("Appear");
            animatorFaHaiDialogue.SetTrigger("Appear");
        }, 1f));
    }

    public void StopDialogue()
    {
        animatorAspectRatioSwitch.SetTrigger("Out");
        animatorDarkenBackground.SetTrigger("Bright");
        animatorXuXianDialogue.SetTrigger("Disappear");
        animatorFaHaiDialogue.SetTrigger("Disappear");
    }

    public void FadeOut()
    {
        animatorFadeScene.SetTrigger("FadeOut");
    }

    // Helper Function for the tutorial dialogue
    public void TutorialDialogueDone()
    {
        CharacterTalking("leftIsTalking", false);
        CharacterTalking("rightIsTalking", false);
        StopDialogue();
        tutorialIntroDialoguePlaying = false;
    }
    
    public void CharacterTalking(string whoIsTalking, bool brightenCharacter)
    {
        if (whoIsTalking == "leftIsTalking")
        {
            //int sortingOrder = _leftCharacterSprite.sortingOrder;
            // Brighten the left character when talking
            if (brightenCharacter)
            {
                _leftCharacterSprite.sortingOrder = 2;        
            }
            // Darken the left character when talking
            else
            {
                _leftCharacterSprite.sortingOrder = 0;
            }
        }
        else if (whoIsTalking == "rightIsTalking")
        {
            //int sortingOrder = _rightCharacterSprite.sortingOrder;
            // Brighten the right character when talking
            if (brightenCharacter)
            {
                _rightCharacterSprite.sortingOrder = 2;        
            }
            // Darken the right character when talking
            else
            {
                _rightCharacterSprite.sortingOrder = 0;
            }
        }
    }

    public void NoDialogue()
    {
        leftCharacter.SetActive(false);
        rightCharacter.SetActive(false);
        animatorDarkenBackground.SetTrigger("StartBright");
    }
    
    //===========================================================
    //                  GameController API
    //===========================================================
    
    [HideInInspector]public bool battleCondition = false;
    void BattleConditionCheck()
    {
        // Switch scenes if player dies
        if (player.GetComponent<Character>().Health_Current <= 0)
        {
            DisableBattleMode();
            
            player.GetComponent<Character>().Health_Current = player.GetComponent<Character>().Health_Total;
            SceneManager.LoadScene("EndScene");
        }
        
        // Switch scene if player wins
        if (enemy.GetComponent<Character>().Health_Current <= 0 && battleCondition)
        {
            DisableBattleMode();

            // player.GetComponent<Character>().Health_Current = player.GetComponent<Character>().Health_Total;
            battleCondition = false;
            SceneManager.LoadScene("StoryIntro");
        }
    }
    
    /*  Function that will setup spawning point for characters group and Camera
     *  Parameters: Argument1:  Target Character Group game object's transform
     *              Argument2:  Target Environment Camera's transform                         
     */
    // public void SetSpawningPoint(Transform characterTransform, Transform environmentCameraTransform)
    // {
    //     SetCharacterPos(characterTransform);
    //     SetCameraPos(environmentCameraTransform);
    // }
    
    /*  Function that will start the battle for testing, assigned deck is required
    *  Parameters:  Argument1:  The next enemy you want to setup 
    *               Argument2 (override): true/false, doesn't matter
    *                                     this version will allow to start battle without deck = 10 cards
    */
    public void StartTheBattle(Character_Basedata enemy, bool overrideVer)
    {
        BattleSystemSetUp(enemy);
    }
    
    void StartTheBattle(Character_Basedata enemy)
    {
        if (_script_DeckSystem.deckToUse.Count == 10)
        {
            //battleCondition = true;
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
    
    // Use API Above and Ignore all function below
    //==========================================================================================================================
    
    //===========================================================
    //                  Helper Functions
    //===========================================================
    
    /* StartTheBattle(Character_Basedata enemy, bool overrideVer) or StartTheBattle(Character_Basedata enemy)
     *  Function that will setup battle system
     *  
     *  Parameters: Argument1:  Target Character Group game object's transform
     *              Argument2:  Target Environment Camera's transform                         
     */
    void BattleSystemSetUp(Character_Basedata enemy)
    {
        // Don't change order of call
        SetCharacter(characterType.enemy,enemy);
        characters.SetActive(true);
        // Implement the character reassignment here
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), true);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_HandManager.SetUp();
        _script_DeckSystem.SetUp();
        _script_BattleController.SetUp();
        _script_EffectDictionary.SetUp();
    }

    /*  Function that will get Enemy From the Enemy List by Name
     *  Parameters: Argument1:  Target Enemy Name
     *  Return:     Character_Basedata an Enemy basedata or error if enemy is not found or not unique
     */
    public Character_Basedata GetCharacter(string name)
    {
        Character_Basedata result;
        result = CharactersList.Where(Basedata => Basedata.name == name).SingleOrDefault();
        if (result != null)
        {
            return result;
        }
        else
        {
            Debug.Log("Error: GetCharacter() in GameController. No such character is found or character is not unique");
            return result;
        }
    }

    // Helper function: Setup for the developer scene/script
    public void DeveloperBattleSetup(string playerName, string enemyName)
    {
        battleCondition = true;
        SetCharacter(characterType.player, GetCharacter(playerName));
        SetCharacter(characterType.enemy, GetCharacter(enemyName));
        characters.SetActive(true);
        
        // Implement the character reassignment here
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), true);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_HandManager.SetUp();
        _script_DeckSystem.SetUp();
        _script_BattleController.SetUp();
        _script_EffectDictionary.SetUp();
    }
    
    // Setup function for tutorial only
    public void TutorialBattleSetup()
    {
        SetCharacter(characterType.enemy, GetCharacter("Peng Hou"));
        SetCharacter(characterType.player, GetCharacter("Xu Xian"));
        characters.SetActive(true);
        
        // Implement the character reassignment here
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), true);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_HandManager.SetUp();
        _script_DeckSystem.SetUp();
        _script_BattleController.SetUp();
        _script_EffectDictionary.SetUp();
    }

    // Helper function to change the player sprite
    public void changePlayerSprite()
    {
        SetCharacter(characterType.player, GetCharacter("Bai Suzhen"));
    }

    public void DisableBattleController()
    {
        _script_BattleController.Clear();
    }
    
    public void DisableBattleMode()
    {
        _script_DeckSystem.deckToUse.Clear();
        _script_DeckSystem.Clear();
        _script_HandManager.Clear();
        _script_BattleController.Clear();
        
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        characters.SetActive(false);
    }

    //  StartTheCamp()
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

    public enum characterType {player, enemy}
    public void SetCharacter(characterType characterTarget, Character_Basedata newCharacter)
    {
        if (characterTarget == characterType.enemy)
        {
            Character enemyCharacter = enemy.GetComponent<Character>();
            enemyCharacter.CharacterData = newCharacter;

            if (enemyCharacter.CharacterData.characterName == "Ink Golem")
            {
                // ??? i don't know why but if the Z isn't set to 50 then it becomes -50, temp fix
                enemy.transform.position = new Vector3(12.61F, -7.66F, 50.0F);
                enemy.transform.localScale = new Vector3(1.02F, 1.02F, 1.02F);
            } 
            if (enemyCharacter.CharacterData.characterName == "Ink Chimera")
            {
                enemy.transform.position = new Vector3(8.84F, -9.1F, 50.0F);
                enemy.transform.localScale = new Vector3(1.38F, 1.38F, 1.38F);
            }
            if (enemyCharacter.CharacterData.characterName == "Zhenniao")
            {
                enemy.transform.position = new Vector3(3.01F, -15.09F, 50.0F);
                enemy.transform.localScale = new Vector3(1.72F, 1.72F, 1.72F);
            }
            enemyCharacter.SetUp();
        }
        else if (characterTarget == characterType.player)
        {
            Character playerCharacter = player.GetComponent<Character>();
            playerCharacter.CharacterData = newCharacter;
            playerCharacter.SetUp();
        }
    }

    // SetSpawningPoint()
    private void SetCharacterPos(Transform targetTrans)
    {
        characters.transform.position = targetTrans.position;
        characters.transform.rotation = targetTrans.rotation;
    }

    // SetSpawningPoint()
    private void SetCameraPos(Transform targetTrans)
    {
        CamerasObj[0].transform.position = targetTrans.position;
        CamerasObj[0].transform.rotation = targetTrans.rotation;
    }

    //                  Helper Function End
    //===========================================================
}
