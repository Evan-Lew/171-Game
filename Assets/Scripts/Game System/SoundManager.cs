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
    [HideInInspector] public static AudioClip sfx_Card_Draw, sfx_Card_Place;
    [HideInInspector] public static AudioClip sfx_Action_01_ThrowStone1, sfx_Action_01_ThrowStone2, sfx_Action_02_Throw_Himself;

    [HideInInspector] public static AudioSource audioSrc;
    //[HideInInspector] public static AudioSource audioSrc_footStep;

    void Start()
    {
        // Loading misc audio files
        sfx_Card_Draw = Resources.Load<AudioClip>("SFX/Card Sounds/Card Draw");
        sfx_Card_Place = Resources.Load<AudioClip>("SFX/Card Sounds/Card Place");
        
        // Loading enemy audio files
        //sfx_Explosion = Resources.Load<AudioClip>("SFX/Explosion");
        sfx_Action_01_ThrowStone1 = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Stone");
        sfx_Action_01_ThrowStone2 = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Stone2");
        sfx_Action_02_Throw_Himself = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Himself");
        
        audioSrc = GetComponent<AudioSource>();
        // audioSrc_footStep = GameObject.Find("SFX/SoundManager/FootStep").GetComponent<AudioSource>();
    }

    // Example call in other script
    // SoundManager.PlaySound("sfx_Explosion", 1);

    // --Parameters--
    // clip: which sound you want to play
    // volumn: how loud you want the sound to be
    public static void PlaySound(string clip, float volumn)
    {
        switch (clip)
        {
            // Card Draw SFX
            case "sfx_Card_Draw":
                audioSrc.clip = sfx_Card_Draw;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;
            
            case "sfx_Card_Place":
                audioSrc.clip = sfx_Card_Place;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;            

            // Golem SFX
            case "sfx_Action_01_ThrowStone1":
                audioSrc.clip = sfx_Action_01_ThrowStone1;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;
            
            case "sfx_Action_01_ThrowStone2":
                audioSrc.clip = sfx_Action_01_ThrowStone2;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;            

            case "sfx_Action_02_Throw_Himself":
                audioSrc.clip = sfx_Action_02_Throw_Himself;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
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
