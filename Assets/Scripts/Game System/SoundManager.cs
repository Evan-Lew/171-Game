using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    //Note: To add anyother audio
    /*           1. put your audio into folder called "Resources/SFX"
     *           2. add load in Void Start()
     *           3. add it into PlaySound() at the end of this script
     *           4. call function in other script with ->  SoundManager.PlaySound("sfx_Explosion", 1)  
     */
    
    // BGM
    [HideInInspector] public static AudioClip bgm_Mountain_Of_Myths;
    
    // Cards
    [HideInInspector] public static AudioClip sfx_Card_Draw, sfx_Card_Place;
    
    // Enemy
    [HideInInspector] public static AudioClip sfx_Action_01_ThrowStone1, sfx_Action_01_ThrowStone2, sfx_Action_02_Throw_Himself;

    // Misc
    [HideInInspector] public static AudioClip sfx_PageFlip;
    
    // AudioSources
    [HideInInspector] public static AudioSource bgmAudioSource;
    [HideInInspector] public static AudioSource sfxAudioSource;

    void Start()
    {
        // Loading background music
        bgm_Mountain_Of_Myths = Resources.Load<AudioClip>("SFX/Background Music/Mountain_Of_Myths");
        
        // Loading card audio files
        sfx_Card_Draw = Resources.Load<AudioClip>("SFX/Card Sounds/Card Draw");
        sfx_Card_Place = Resources.Load<AudioClip>("SFX/Card Sounds/Card Place");
        
        // Loading enemy audio files
        sfx_Action_01_ThrowStone1 = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Stone");
        sfx_Action_01_ThrowStone2 = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Stone2");
        sfx_Action_02_Throw_Himself = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Himself");
        
        // Loading misc audio files
        sfx_PageFlip = Resources.Load<AudioClip>("SFX/Misc/PageFlip");
        
        // Get AudioSource components
        sfxAudioSource = GetComponent<AudioSource>();
        bgmAudioSource = GameObject.Find("Sound Manager/BGM Sound Manager").GetComponent<AudioSource>();
    }

    // --Example call in other script--
    // SoundManager.PlaySound("sfx_Explosion", 1);

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
                bgmAudioSource.PlayOneShot(bgmAudioSource.clip);
                break;

            //----------Misc SFX----------
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

            //----------Enemy SFX----------
            // Golem SFX
            case "sfx_Action_01_ThrowStone1":
                sfxAudioSource.clip = sfx_Action_01_ThrowStone1;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            case "sfx_Action_01_ThrowStone2":
                sfxAudioSource.clip = sfx_Action_01_ThrowStone2;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;            

            case "sfx_Action_02_Throw_Himself":
                sfxAudioSource.clip = sfx_Action_02_Throw_Himself;
                sfxAudioSource.volume = volumn;
                sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                break;
            
            //----------Enemy SFX----------
            case "sfx_PageFlip":
                sfxAudioSource.clip = sfx_PageFlip;
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
