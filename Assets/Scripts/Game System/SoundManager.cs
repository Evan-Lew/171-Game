using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Note: To add any other audio
    /*           1. put your audio into folder called "Resources/SFX"
     *           2. add load in Void Start()
     *           3. add it into PlaySound() at the end of this script
     *           4. call function in other script with ->  SoundManager.PlaySound("sfx_Explosion", 1)  
     */
    
    // BGM
    [HideInInspector] public static AudioClip bgm_Mountain_Of_Myths, bgm_Mountain_Ambient, bgm_River_Ambient;
    
    // Cards
    [HideInInspector] public static AudioClip sfx_Card_Draw, sfx_Card_Place, sfx_Card_Pick;
    
    // Player Silver/Purple Cards
    [HideInInspector] public static AudioClip sfx_Coin_Drop, sfx_Hiss, sfx_Hit, sfx_Venom, sfx_Swing, sfx_Crunch, 
        sfx_Wisdom, sfx_Stab, sfx_Fortune, sfx_Tea_Pour, sfx_Gavel, sfx_Multiple_Splash;

    // Player Jade Cards
    [HideInInspector] public static AudioClip sfx_Spirit;
    
    // Enemy Cards
    [HideInInspector] public static AudioClip sfx_Action_01_Throw_Stone, sfx_Action_02_Body_Slam, sfx_Action_03_Stubborn, 
        sfx_Action_04_Drain, sfx_Action_Rock_Smash, sfx_Action_Reverberate, sfx_Action_Breath, sfx_Action_Monsterize, 
        sfx_Action_Breeze, sfx_Action_Whoosh, sfx_Action_Cyclone;

    // Misc
    [HideInInspector] public static AudioClip sfx_Page_Flip, sfx_Scroll_Open, sfx_Transition, sfx_Cough;
    
    // AudioSources
    [HideInInspector] public static AudioSource bgmAudioSource;
    [HideInInspector] public static AudioSource sfxAudioSource;

    void Start()
    {
        // Loading background music
        bgm_Mountain_Of_Myths = Resources.Load<AudioClip>("SFX/Background Music/Mountain_Of_Myths");
        bgm_Mountain_Ambient = Resources.Load<AudioClip>("SFX/Background Music/Mountain_Ambient");
        bgm_River_Ambient = Resources.Load<AudioClip>("SFX/Background Music/River_Ambient");
        
        // Loading card audio files
        sfx_Card_Draw = Resources.Load<AudioClip>("SFX/Card Sounds/Card_Draw");
        sfx_Card_Place = Resources.Load<AudioClip>("SFX/Card Sounds/Card_Place");
        sfx_Card_Pick = Resources.Load<AudioClip>("SFX/Card Sounds/Card_Pick");
        
        // Loading player audio files
        sfx_Coin_Drop = Resources.Load<AudioClip>("SFX/Player/Coin_Drop");
        sfx_Hiss = Resources.Load<AudioClip>("SFX/Player/Hiss");
        sfx_Hit = Resources.Load<AudioClip>("SFX/Player/Hit");
        sfx_Venom = Resources.Load<AudioClip>("SFX/Player/Venom");
        sfx_Swing = Resources.Load<AudioClip>("SFX/Player/Swing");
        sfx_Crunch = Resources.Load<AudioClip>("SFX/Player/Crunch");
        sfx_Wisdom = Resources.Load<AudioClip>("SFX/Player/Wisdom");
        sfx_Spirit = Resources.Load<AudioClip>("SFX/Player/Spirit");
        sfx_Stab = Resources.Load<AudioClip>("SFX/Player/Stab");
        sfx_Fortune = Resources.Load<AudioClip>("SFX/Player/Fortune");
        sfx_Tea_Pour = Resources.Load<AudioClip>("SFX/Player/Tea_Pour");
        sfx_Gavel = Resources.Load<AudioClip>("SFX/Player/Gavel");
        sfx_Multiple_Splash = Resources.Load<AudioClip>("SFX/Player/Multiple_Splash");
        
        // Loading enemy audio files
        sfx_Action_01_Throw_Stone = Resources.Load<AudioClip>("SFX/Enemy/Throw_Stone");
        sfx_Action_02_Body_Slam = Resources.Load<AudioClip>("SFX/Enemy/Body_Slam");
        sfx_Action_03_Stubborn = Resources.Load<AudioClip>("SFX/Enemy/Snarl");
        sfx_Action_04_Drain = Resources.Load<AudioClip>("SFX/Enemy/Wind_Blow");
        sfx_Action_Rock_Smash = Resources.Load<AudioClip>("SFX/Enemy/Rock_Smash");
        sfx_Action_Reverberate = Resources.Load<AudioClip>("SFX/Enemy/Reverberate");
        sfx_Action_Breath = Resources.Load<AudioClip>("SFX/Enemy/Breath");
        sfx_Action_Monsterize = Resources.Load<AudioClip>("SFX/Enemy/Monsterize");
        sfx_Action_Breeze = Resources.Load<AudioClip>("SFX/Enemy/Breeze");
        sfx_Action_Whoosh = Resources.Load<AudioClip>("SFX/Enemy/Whoosh");
        sfx_Action_Cyclone = Resources.Load<AudioClip>("SFX/Enemy/Cyclone");

        // Loading misc audio files
        sfx_Page_Flip = Resources.Load<AudioClip>("SFX/Misc/Page_Flip");
        sfx_Scroll_Open = Resources.Load<AudioClip>("SFX/Misc/Scroll_Open");
        sfx_Transition = Resources.Load<AudioClip>("SFX/Misc/Transition");
        sfx_Cough = Resources.Load<AudioClip>("SFX/Misc/Cough");
        
        // Get AudioSource components
        sfxAudioSource = GetComponent<AudioSource>();
        bgmAudioSource = GameObject.Find("Sound Manager/BGM Sound Manager").GetComponent<AudioSource>();
    }

    // --Example call in other script--
    // SoundManager.PlaySound("sfx_Explosion", 1f);

    // --Parameters--
    // clip: which sound you want to play
    // volumn: how loud you want the sound to be
    public static void PlaySound(string clip, float volumn)
    {
        switch (clip)
        {
            //----------BGM SFX----------
            case "bgm_Mountain_Of_Myths":
                bgmAudioSource.clip = bgm_Mountain_Of_Myths;
                bgmAudioSource.volume = volumn;
                bgmAudioSource.Play();
                break;
            
            case "bgm_Mountain_Ambient":
                bgmAudioSource.clip = bgm_Mountain_Ambient;
                bgmAudioSource.volume = volumn;
                bgmAudioSource.Play();
                break;
            
            case "bgm_River_Ambient":
                bgmAudioSource.clip = bgm_River_Ambient;
                bgmAudioSource.volume = volumn;
                bgmAudioSource.Play();
                break;

            //----------Card SFX----------
            // Card Draw SFX
            case "sfx_Card_Draw":
                sfxAudioSource.clip = sfx_Card_Draw;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Card_Place":
                sfxAudioSource.clip = sfx_Card_Place;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Card_Pick":
                sfxAudioSource.clip = sfx_Card_Pick;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;

            //----------Player SFX----------
            case "sfx_Coin_Drop":
                sfxAudioSource.clip = sfx_Coin_Drop;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Hiss":
                sfxAudioSource.clip = sfx_Hiss;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Hit":
                sfxAudioSource.clip = sfx_Hit;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Venom":
                sfxAudioSource.clip = sfx_Venom;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Swing":
                sfxAudioSource.clip = sfx_Swing;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Crunch":
                sfxAudioSource.clip = sfx_Crunch;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Wisdom":
                sfxAudioSource.clip = sfx_Wisdom;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Spirit":
                sfxAudioSource.clip = sfx_Spirit;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Stab":
                sfxAudioSource.clip = sfx_Stab;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Fortune":
                sfxAudioSource.clip = sfx_Fortune;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Tea_Pour":
                sfxAudioSource.clip = sfx_Tea_Pour;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Gavel":
                sfxAudioSource.clip = sfx_Gavel;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Multiple_Splash":
                sfxAudioSource.clip = sfx_Multiple_Splash;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            //----------Enemy SFX----------
            // Golem SFX
            case "sfx_Action_01_Throw_Stone":
                sfxAudioSource.clip = sfx_Action_01_Throw_Stone;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;

            case "sfx_Action_02_Body_Slam":
                sfxAudioSource.clip = sfx_Action_02_Body_Slam;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_03_Stubborn":
                sfxAudioSource.clip = sfx_Action_03_Stubborn;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_04_Drain":
                sfxAudioSource.clip = sfx_Action_04_Drain;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_Rock_Smash":
                sfxAudioSource.clip = sfx_Action_Rock_Smash;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;

            case "sfx_Action_Reverberate":
                sfxAudioSource.clip = sfx_Action_Reverberate;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_Breath":
                sfxAudioSource.clip = sfx_Action_Breath;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_Monsterize":
                sfxAudioSource.clip = sfx_Action_Monsterize;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_Breeze":
                sfxAudioSource.clip = sfx_Action_Breeze;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_Whoosh":
                sfxAudioSource.clip = sfx_Action_Whoosh;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_Cyclone":
                sfxAudioSource.clip = sfx_Action_Cyclone;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            //----------Misc SFX----------
            case "sfx_Page_Flip":
                sfxAudioSource.clip = sfx_Page_Flip;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Scroll_Open":
                sfxAudioSource.clip = sfx_Scroll_Open;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Transition":
                sfxAudioSource.clip = sfx_Transition;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Cough":
                sfxAudioSource.clip = sfx_Cough;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            ////footstep will be paused when it's not moving, so I use another audio source to avoid pause other clips at same time
                //case "sfx_Footstep":
                //    audioSrc_footStep.clip = sfx_Footstep;
                //    audioSrc_footStep.volume = volumn;
                //    audioSrc_footStep.Play();
                //    break;
        }
    }
}
