using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [SerializeField] CameraUtil _script_CameraUtil;
    [SerializeField] BattleController _script_BattleController;
    [SerializeField] HandManager _script_HandManager;
    [SerializeField] EffectDictionary _script_EffectDictionary;
    [SerializeField] PrioritySystem _script_PrioritySystem;
    [SerializeField] DeckSystem _script_DeckSystem;
    [SerializeField] DeckEditSystem _script_DeckEditSystem;

    [SerializeField] GameObject characters;
    [SerializeField] GameObject player, enemy;
    
    [Header("Animator Controllers")]
    [SerializeField] Animator animatorFadeScene;
    [SerializeField] private Animator animatorAspectRatioSwitch, animatorDarkenBackground, animatorXuXuanDialogue, animatorFaHaiDialogue, animatorBaiSuzhenDialogue, animatorBiZiDialogue, animatorXuanWuDialogue;

    [Header("Story Background Lists and Story Animators")]
    public int storyScenesLeft = 4;
    public int scenesPlayed = 0;
    [SerializeField] List<Animator> animatorScreenWipesList;
    [SerializeField] public List<GameObject> storyBackgroundsList;

    [Header("Background Variables")] 
    public List<Texture> backgroundsList = new();
    public GameObject currBackground;
    public GameObject currForeground;

    [Header("Cloud Objects")] 
    [SerializeField] private Animator animatorCloud;
    [SerializeField] private GameObject clouds;
    
    // Dialogue GameObjects
    [Header("Characters Talking")]
    [SerializeField] GameObject xuXianGameObj;
    [SerializeField] GameObject faHaiGameObj;
    [SerializeField] GameObject baiSuzhenGameObj;
    [SerializeField] GameObject biZiGameObj;
    [SerializeField] GameObject xuanWuGameObj;
    
    private SpriteRenderer _xuXianSprite;
    private SpriteRenderer _faHaiSprite;
    private SpriteRenderer _baiSuzhenSprite;
    private SpriteRenderer _biZiSprite;
    private SpriteRenderer _xuanWuSprite;
    
    // List for all the characters (don't know what this is for -Evan)
    public List<Character_Basedata> CharactersList = new();
    Character_Basedata newEnemy;

    // List to keep track of player sprites
    public List<GameObject> playerSpriteGameObjects = new();
    public List<Animator> animatorPlayerList = new();
    public Animator currAnimatorPlayer;
    
    // List to keep track of all enemy sprites
    public List<GameObject> enemySpriteGameObjects = new();
    public List<Animator> animatorEnemyList = new();
    public Animator currAnimatorEnemy;
    
    [Header("Don't change order!")]
    [SerializeField] List<GameObject> CamerasObj;
    
    // --Not used anymore--
    // Set position -- character and enemy
    // [SerializeField] List<GameObject> CharacterSpawningPoint_List = new();
    // [SerializeField] List<GameObject> CameraSpawningPoint_List = new();
    // public GameObject TargetCharacterPos;
    // public GameObject TargetCameraPos;
    // Flag for changing level (NOT USED)
    [HideInInspector] public bool isDeckELevel = true;
    [HideInInspector] public bool isStartLevel = false;
    
    public static GameController instance;
    [HideInInspector] public bool enableMouseEffectOnCard = true;
    
    // Tutorial Variables
    [HideInInspector] public bool tutorialIntroDialoguePlaying = false;
    [HideInInspector] public bool tutorialOutroDialoguePlaying = false;
    [HideInInspector] public bool tutorialLevelEnd = false;
    
    private void Awake()
    {
        instance = this;
        characters.SetActive(false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), false);
        
        // Assign sprites used for the dialogue
        _xuXianSprite = xuXianGameObj.GetComponent<SpriteRenderer>();
        _faHaiSprite = faHaiGameObj.GetComponent<SpriteRenderer>();
        _baiSuzhenSprite = baiSuzhenGameObj.GetComponent<SpriteRenderer>();
        _biZiSprite = biZiGameObj.GetComponent<SpriteRenderer>();
        _xuanWuSprite = xuanWuGameObj.GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("Space");
        //     DisableBattleMode(true);
        //     
        // }
        
        //BattleConditionCheck();
        
        // if (isDeckELevel)
        // {
        //     isDeckELevel = false;
        //     StartTheCamp();
        // }
        // else if (isStartLevel)
        // {
        //     isStartLevel = false;
        // }
        //
        // //--Legacy: Not used--
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartTheCamp();
        }
        
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     SoundManager.PlaySound("bgm_Mountain_Of_Myths", 0.1f);
        //     setupFlag = true;
        //     if (setupFlag)
        //     {
        //         StartTheBattle(GetCharacter("Ink Golem"), true);
        //         setupFlag = false;
        //     }
        // }
        
    }
    
    //=============================================================================================
    //                  Dialogue Helper Functions
    //=============================================================================================
    
    // Helper function: Start dialogue at the start of a scene
    public void StartDialogue(string sceneName)
    {
        animatorFadeScene.SetTrigger("FadeIn");
        

        if (sceneName == "Tutorial")
        {
            animatorAspectRatioSwitch.SetTrigger("StartWithRatio");
            animatorDarkenBackground.SetTrigger("StartDark");
            xuXianGameObj.SetActive(true);
            faHaiGameObj.SetActive(true);
            animatorXuXuanDialogue.SetTrigger("OnScreenMiddleLeft");
            animatorFaHaiDialogue.SetTrigger("OnScreenRight");
        }
        else if (sceneName == "Village")
        {
            // Start animations for starting the scene by itself
            animatorAspectRatioSwitch.SetTrigger("In");
            animatorDarkenBackground.SetTrigger("Dark");
            
            // Activate character objs
            xuXianGameObj.SetActive(true);
            faHaiGameObj.SetActive(true);
            baiSuzhenGameObj.SetActive(true);
            
            // Start character animations
            animatorXuXuanDialogue.SetTrigger("AppearLeftSide");
            animatorFaHaiDialogue.SetTrigger("AppearLeft");
            animatorBaiSuzhenDialogue.SetTrigger("AppearRight");
        }
        else if (sceneName == "Forest")
        {
            // Start animations for starting the scene by itself
            animatorAspectRatioSwitch.SetTrigger("In");
            animatorDarkenBackground.SetTrigger("Dark");
            
            baiSuzhenGameObj.SetActive(true);
            biZiGameObj.SetActive(true);
            
            animatorBaiSuzhenDialogue.SetTrigger("AppearLeft");
            animatorBiZiDialogue.SetTrigger("OffScreenRight");
        }
        else if (sceneName == "Cave")
        {
            // Start animations for starting the scene by itself
            animatorAspectRatioSwitch.SetTrigger("In");
            animatorDarkenBackground.SetTrigger("Dark");
            
            baiSuzhenGameObj.SetActive(true);
            biZiGameObj.SetActive(true);
            xuanWuGameObj.SetActive(true);
            
            animatorBaiSuzhenDialogue.SetTrigger("AppearLeft");
            animatorBiZiDialogue.SetTrigger("AppearMiddle");
            animatorXuanWuDialogue.SetTrigger("OnScreenRight");
        }
    }

    // Helper function: Restart the dialogue during an active scene
    public void RestartDialogue(string sceneName)
    {
        animatorAspectRatioSwitch.SetTrigger("In");
        animatorDarkenBackground.SetTrigger("Dark");
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            if (sceneName == "Tutorial")
            {
                animatorXuXuanDialogue.SetTrigger("AppearMiddleLeft");
                animatorFaHaiDialogue.SetTrigger("AppearRight");    
            }
        }, 1f));
    }

    // Helper function: Pause the dialogue bc you will restart later in the same scene
    public void PauseDialogue(string sceneName)
    {
        animatorAspectRatioSwitch.SetTrigger("Out");
        

        if (sceneName == "Tutorial")
        {
            animatorDarkenBackground.SetTrigger("Bright");
            CharacterTalking("Xu Xian", false);
            CharacterTalking("Fa Hai", false);
            if (xuXianGameObj.activeSelf && faHaiGameObj.activeSelf)
            {
                animatorXuXuanDialogue.SetTrigger("DisappearMiddleLeft");
                animatorFaHaiDialogue.SetTrigger("DisappearRight");
            }    
        }
    }

    // Helper function: End dialogue to set it back to it's original state
    public void EndDialogue(string sceneName)
    {
        if (sceneName == "Tutorial" || sceneName == "Village")
        {
            // Reset the characters
            // animatorXuXuanDialogue.SetTrigger("OffScreenLeft");
            // animatorFaHaiDialogue.SetTrigger("OffScreenRight");
            xuXianGameObj.SetActive(false);
            faHaiGameObj.SetActive(false);
            baiSuzhenGameObj.SetActive(false);
        }
        else if (sceneName == "Cave")
        {
            xuXianGameObj.SetActive(false);
            faHaiGameObj.SetActive(false);
            baiSuzhenGameObj.SetActive(false);
            biZiGameObj.SetActive(false);
            xuanWuGameObj.SetActive(false);
        }
        else if (sceneName == "BattleLevel")
        {
            animatorFadeScene.SetTrigger("ClearScreen");
            animatorAspectRatioSwitch.SetTrigger("Out");
            animatorDarkenBackground.SetTrigger("Bright");
        }
        
        //animatorFadeScene.SetTrigger("ClearScreen");
        //animatorAspectRatioSwitch.SetTrigger("Out");
        //animatorDarkenBackground.SetTrigger("Bright");
    }

    public void FadeIn()
    {
        animatorFadeScene.SetTrigger("FadeIn");
    }
    
    public void FadeOut()
    {
        animatorFadeScene.SetTrigger("FadeOut");
    }

    public void RatioOutandBrightenScreen()
    {
        animatorAspectRatioSwitch.SetTrigger("Out");
        animatorDarkenBackground.SetTrigger("Bright");
    }

    public void UIAnimationsOffScreen()
    {
        animatorFadeScene.SetTrigger("ClearScreen");
        animatorFadeScene.ResetTrigger("FadeOut");
        animatorAspectRatioSwitch.SetTrigger("NoRatio");
        animatorDarkenBackground.SetTrigger("StartBright");
    }
    
    // For developer level to have no fade in
    public void ClearScreen()
    {
        animatorFadeScene.SetTrigger("ClearScreen");
    }

    public void ResetAnimations()
    {
        animatorAspectRatioSwitch.ResetTrigger("Out");
        animatorDarkenBackground.ResetTrigger("Bright");
    }
    
    //=============================================================================================
    //                  Tutorial Helper Functions
    //=============================================================================================
    
    // Helper Function: for the tutorial dialogue
    public void TutorialIntroDialogueDone()
    {
        PauseDialogue("Tutorial");
        tutorialIntroDialoguePlaying = false;
    }
    
    public void TutorialOutroDialogueDone()
    {
        PauseDialogue("Tutorial");
        tutorialOutroDialoguePlaying = false;
    }

    //=============================================================================================
    //                  Story Village Level Helper Functions
    //=============================================================================================
    public void TurnToSnakeDialogue()
    {
        animatorBaiSuzhenDialogue.SetTrigger("TurnToSnake");
    }

    public void FlashOfLightAppearDialogue()
    {
        animatorDarkenBackground.SetTrigger("FlashOfLightAppear");
    }

    public void FlashOfLightDisappearDialogue()
    {
        animatorDarkenBackground.SetTrigger("FlashOfLightDisappear");
    }

    public void HurtBackgroundDialogue()
    {
        animatorDarkenBackground.SetTrigger("Hurt");
    }
    
    public void XuXianMoveRightDialogue()
    {
        animatorXuXuanDialogue.SetTrigger("MoveRight");
    }

    public void XuXianHurtDialogue()
    {
        animatorXuXuanDialogue.SetTrigger("Hurt");
    }

    public void BaiSuzhenRunDialogue()
    {
        animatorBaiSuzhenDialogue.SetTrigger("Run");
    }

    public void FaHaiAttackDialogue()
    {
        animatorFaHaiDialogue.SetTrigger("Attack");
    }

    public void FaHaiDisappearLeftDialogue()
    {
        animatorFaHaiDialogue.SetTrigger("DisappearLeft");
    }
    
    //=============================================================================================
    //                  Story Forest Level Helper Functions
    //=============================================================================================

    public void BiZiAppearRightDialogue()
    {
        animatorBiZiDialogue.SetTrigger("AppearRight");
    }

    public void BiZiBaiSuzhenDisappearRightDialogue()
    {
        animatorBiZiDialogue.SetTrigger("DisappearRight");
        animatorBaiSuzhenDialogue.SetTrigger("DisappearLeftToRight");
    }

    public void BaiSuzhenOffScreenLeft()
    {
        baiSuzhenGameObj.SetActive(false);
        animatorBaiSuzhenDialogue.SetTrigger("OffScreenLeft");
    }
    
    //=============================================================================================
    //                  Story Cave Level Helper Functions
    //=============================================================================================

    public void XuanWuDisappearLeftDialogue()
    {
        animatorXuanWuDialogue.SetTrigger("DisappearRight");
    }

    public void BiZiLookAroundDialogue()
    {
        animatorBiZiDialogue.SetTrigger("LookAround");
    }

    public void BiZiBaiSuzhenDisappearLeftDialogue()
    {
        animatorBiZiDialogue.SetTrigger("DisappearMiddle");
        animatorBaiSuzhenDialogue.SetTrigger("DisappearLeft");
    }
    
    //=============================================================================================

    // Helper Function: Call this function to bring a sprite above the darken background during dialogue
    public void CharacterTalking(string character, bool brightenCharacter)
    {
        // Xu Xian
        if (character == "Xu Xian")
        {
            // Brighten the Xu Xian
            if (brightenCharacter)
            {
                _xuXianSprite.sortingOrder = 2;
                //SoundManager.PlaySound("sfx_Calligraphy", 0.1f);          
            }
            // Darken the Xu Xian
            else
            {
                _xuXianSprite.sortingOrder = 0;
            }
        }
        else if (character == "Fa Hai")
        {
            if (brightenCharacter)
            {
                _faHaiSprite.sortingOrder = 2;
                //SoundManager.PlaySound("sfx_Calligraphy", 0.1f);            
            }
            else
            {
                _faHaiSprite.sortingOrder = 0;
            }
        }
        else if (character == "Bai Suzhen")
        {
            if (brightenCharacter)
            {
                _baiSuzhenSprite.sortingOrder = 2;
                //SoundManager.PlaySound("sfx_Calligraphy", 0.1f);            
            }
            else
            {
                _baiSuzhenSprite.sortingOrder = 0;
            }
        }
        else if (character == "Bi Zi")
        {
            if (brightenCharacter)
            {
                _biZiSprite.sortingOrder = 2;
                //SoundManager.PlaySound("sfx_Calligraphy", 0.1f);            
            }
            else
            {
                _biZiSprite.sortingOrder = 0;
            }
        }
        else if (character == "Xuan Wu")
        {
            if (brightenCharacter)
            {
                _xuanWuSprite.sortingOrder = 2;
                //SoundManager.PlaySound("sfx_Calligraphy", 0.1f);            
            }
            else
            {
                _xuanWuSprite.sortingOrder = 0;
            }
        }
    }

    //=============================================================================================
    //                  GameController API
    //=============================================================================================
    
    [HideInInspector]public bool battleCondition = false;
    void BattleConditionCheck()
    {
        // Switch scenes if player dies
        if (player.GetComponent<Character>().Health_Current <= 0)
        {
            DisableBattleMode(true);
            
            // player.GetComponent<Character>().Health_Current = player.GetComponent<Character>().Health_Total;
            SceneManager.LoadScene("EndScene");
        }
        
        // Switch scene if player wins
        if (enemy.GetComponent<Character>().Health_Current <= 0 && battleCondition)
        {
            DisableBattleMode(true);

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

    /*  Function that will start camp view
    *  Parameters:  void
    */
    void StartTheCamp()
    {
        CampSystemSetUp();
    }
    
    //                GameController API End
    //===========================================================
    
    // Use API Above and Ignore all function below
    //=============================================================================================
    
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
        enableMouseEffectOnCard = false;
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
        
        // This is to avoid an index error when drawing cards at the beginning of a battle
        StartCoroutine(CoroutineUtil.instance.WaitNumSeconds(() =>
        {
            enableMouseEffectOnCard = true;
        }, 2.25f));
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

    // Setup function for tutorial only
    public void TutorialBattleSetup()
    {
        SetCharacter(characterType.enemy, GetCharacter("Peng Hou"));
        SetCharacter(characterType.player, GetCharacter("Xu Xuan"));
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
        SetCharacter(characterType.player, GetCharacter("Bai She Zhuan"));
    }

    public void DisableBattleController()
    {
        _script_BattleController.Clear();
    }
    
    public void DisableBattleMode(bool clearDeck)
    {
        if (clearDeck)
        {
            _script_DeckSystem.deckToUse.Clear();
            _script_DeckSystem.Clear();    
        }
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
    public void CampSystemSetUp()
    {
        characters.SetActive(false);
        //_script_BattleController.Clear();
        _script_DeckSystem.Clear();
        _script_HandManager.Clear();
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Battle Camera").SingleOrDefault().GetComponent<Camera>(), false);
        _script_CameraUtil.SetUIActive(CamerasObj.Where(obj => obj.name == "UI Camp Camera").SingleOrDefault().GetComponent<Camera>(), true);
    }

    public void StartBattleLevelButton()
    {
        SceneManager.LoadScene("BattleLevel");
    }

    public enum characterType {player, enemy}
    
    // Change character basedata
    public void SetCharacter(characterType characterTarget, Character_Basedata newCharacter)
    {
        if (characterTarget == characterType.enemy)
        {
            Character enemyCharacter = enemy.GetComponent<Character>();
            enemyCharacter.CharacterData = newCharacter;
            
            if (enemyCharacter.CharacterData.characterName == "Peng Hou")
            {
                // If the Z isn't set to 50 then it becomes -50
                enemy.transform.position = new Vector3(4F, -12.75F, 50.0F);
                enemy.transform.localScale = new Vector3(1.7F, 1.7F, 1.7F);
            } 
            else if (enemyCharacter.CharacterData.characterName == "Ink Golem")
            {
                enemy.transform.position = new Vector3(8F, -12F, 50.0F);
                enemy.transform.localScale = new Vector3(1.4F, 1.4F, 1.4F);
            } 
            else if (enemyCharacter.CharacterData.characterName == "Ink Chimera")
            {
                enemy.transform.position = new Vector3(5F, -12.75F, 50.0F);
                enemy.transform.localScale = new Vector3(1.7F, 1.7F, 1.7F);
            }
            else if (enemyCharacter.CharacterData.characterName == "Zhenniao")
            {
                enemy.transform.position = new Vector3(0F, -15F, 50.0F);
                enemy.transform.localScale = new Vector3(1.85F, 1.85F, 1.85F);
            }
            else if (enemyCharacter.CharacterData.characterName == "Xiao Qing")
            {
                enemy.transform.position = new Vector3(22F, -12F, 50.0F);
                enemy.transform.localScale = new Vector3(1.25F, 1.25F, 1.25F);
            }
            else if (enemyCharacter.CharacterData.characterName == "Fa Hai")
            {
                enemy.transform.position = new Vector3(19F, -12F, 50.0F);
                enemy.transform.localScale = new Vector3(1.3F, 1.3F, 1.3F);
            }
            // Not used
            else if (enemyCharacter.CharacterData.characterName == "Stone Rui Shi")
            {
                enemy.transform.position = new Vector3(5.3F, -14.5F, 50.0F);
                enemy.transform.localScale = new Vector3(1.78F, 1.78F, 1.78F);
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

    // Helper Function to change the background
    public void ChangeBackground(string newBackgroundName)
    {
         Texture background = backgroundsList[0];
        
         for (int i = 0; i < backgroundsList.Count; i++)
         {
             if (newBackgroundName == backgroundsList[i].name)
             {
                 background = backgroundsList[i];
                 
                 // Edge Case: To set the foreground for the Mountaintop
                 if (newBackgroundName == "Mountaintop_BG")
                 {
                     currForeground.GetComponent<RawImage>().texture = backgroundsList[7];
                     clouds.SetActive(true);
                 }
                 else
                 {
                     currForeground.GetComponent<RawImage>().texture = backgroundsList[0];
                     clouds.SetActive(false);
                 }
             }
         }
         currBackground.GetComponent<RawImage>().texture = background;
    }

    public void ChangeStoryBackground(int currStoryBackgroundNum)
    {
        animatorScreenWipesList[currStoryBackgroundNum].SetTrigger("Wipe");
        // currBackground.GetComponent<RawImage>().texture = storyBackgroundsList[currStoryBackgroundNum];
        currForeground.GetComponent<RawImage>().texture = backgroundsList[0];
        clouds.SetActive(false);
    }
    
    
    //                  Helper Function End
    //===========================================================
}
