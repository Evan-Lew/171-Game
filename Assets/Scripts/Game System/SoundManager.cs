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


    [HideInInspector] public static AudioClip sfx_Action_01_ThrowStone, sfx_Action_02_Throw_Himself;
    //[HideInInspector] public static AudioClip sfx_GoodClueMonster, sfx_BadClueMonster;

    [HideInInspector]public static AudioSource audioSrc;
    //[HideInInspector]public static AudioSource audioSrc_footStep;
    //[HideInInspector] public static AudioSource audioScr_Monster;

    void Start()
    {
        //loading
        //sfx_Explosion = Resources.Load<AudioClip>("SFX/Explosion");
        sfx_Action_01_ThrowStone = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Stone");
        sfx_Action_02_Throw_Himself = Resources.Load<AudioClip>("SFX/Enemy/Golem/Throw_Himself"); 

      

        audioSrc = GetComponent<AudioSource>();
        //audioSrc_footStep = GameObject.Find("SFX/SoundManager/FootStep").GetComponent<AudioSource>();
        //audioScr_Monster = GameObject.Find("SFX/SoundManager/Monster").GetComponent<AudioSource>();

    }

    //example call in other script
    //SoundManager.PlaySound("sfx_Explosion", 1);



    //clip: which sound u want to play
    //volumn: how loud u want the sound want to be
    public static void PlaySound(string clip, float volumn)
    {
        switch (clip)
        {
            case "sfx_Action_01_ThrowStone":
                audioSrc.clip = sfx_Action_01_ThrowStone;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

            case "sfx_Action_02_Throw_Himself":
                audioSrc.clip = sfx_Action_02_Throw_Himself;
                audioSrc.volume = volumn;
                audioSrc.PlayOneShot(audioSrc.clip);
                break;

                //case "sfx_WilhemScream":
                //    audioSrc.clip = sfx_WilhemScream;
                //    audioSrc.volume = volumn;
                //    audioSrc.PlayOneShot(audioSrc.clip);
                //    break;

                //case "sfx_Pickup":
                //    audioSrc.clip = sfx_Pickup;
                //    audioSrc.volume = volumn;
                //    audioSrc.PlayOneShot(audioSrc.clip);
                //    break;

                ////footstep will be paused when it's not moving, so I use another audio source to avoid pause other clips at same time
                //case "sfx_Footstep":
                //    audioSrc_footStep.clip = sfx_Footstep;
                //    audioSrc_footStep.volume = volumn;
                //    audioSrc_footStep.Play();
                //    break;

                ////monster

                //case "sfx_GoodClueMonster":
                //    audioScr_Monster.clip = sfx_GoodClueMonster;
                //    audioScr_Monster.volume = volumn;
                //    audioScr_Monster.PlayOneShot(audioScr_Monster.clip);
                //    break;

                //case "sfx_BadClueMonster":
                //    audioScr_Monster.clip = sfx_BadClueMonster;
                //    audioScr_Monster.volume = volumn;
                //    audioScr_Monster.PlayOneShot(audioScr_Monster.clip);
                //    break;
        }
    }


}
