using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class CreditsMenu : MonoBehaviour
{
    public TMP_Text titleText;
    [SerializeField] private GameObject titleObj;
    public TMP_Text leftBodyText;
    [SerializeField] private GameObject leftBodyObj;
    public TMP_Text rightBodyText;
    [SerializeField] private GameObject rightBodyObj;
    public TMP_Text middleBodyText;
    [SerializeField] private GameObject middleBodyObj;
    private int pageCount = 0;

    private void Start()
    {
        SoundManager.PlaySound("bgm_River_Ambient", 0.1f);
    }

    public void SetUp()
    {
        titleText = titleObj.GetComponent<TMP_Text>();
        leftBodyText = leftBodyObj.GetComponent<TMP_Text>();
        rightBodyText = rightBodyObj.GetComponent<TMP_Text>();
        middleBodyText = middleBodyObj.GetComponent<TMP_Text>();
    }

    public void ReturnToMainMenu()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        SoundManager.bgmAudioSource.Stop();
        SceneManager.LoadScene("MainMenu");
    }
    
    public void Next()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 0.1f);
        pageCount += 1;
        if (pageCount == 0)
        {
            titleText.text = "Team 10 - 171";
            leftBodyText.text = "Producer<br>Programmers<br><br>Art Director<br>Artists<br><br>Backgrounds<br>Characters<br>Writers<br>Sound<br>Special Thanks";
            rightBodyText.text = "Evan Lew<br>Andy Eng, Alex Xie,\nEvan, Derek Tran, Chris Pau<br>Ed Gopez<br>Angela Yim, Lou Yonzon,\nLee Coronilia<br>Lee<br>Angela & Lou<br>Lee & Ed<br>Evan & Derek<br>Steven Ji & Laihong Xu";
            middleBodyText.text = "";
        }
        else if (pageCount == 1)
        {
            titleText.text = "Research & Fonts";
            middleBodyText.text = "https://en.wikipedia.org/wiki/Legend_of_the_White_Snake<br><br>https://youtu.be/eEeeClBoqK0<br><br>https://youtu.be/mO6eMTKalRE<br><br>Used Nerko One and Cavaet fonts";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 2)
        {
            titleText.text = "Art Style & Costume Inspirations";
            middleBodyText.text = "https://www.youtube.com/watch?v=8y9QnS_tMkY<br>http://www.asean-china-center.org/english/2010-05/26/c_13315960.htm<br>http://www.rjcc.or.kr/journal/article.php?code=37890<br>https://www.chinasilkmuseum.com/zggd/list_21.aspx";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 3)
        {
            titleText.text = "Unity Packages";
            middleBodyText.text = "<u>Particles</u><br>https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-pack-vol-1-231405<br>https://assetstore.unity.com/packages/vfx/particles/spells/100-special-skills-effects-pack-171146<br><u>Sound Effects</u><br>https://assetstore.unity.com/packages/audio/sound-fx/universal-sound-fx-17256#description";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 4)
        {
            titleText.text = "Music Used";
            middleBodyText.text = "https://www.youtube.com/watch?v=mqou-93wd7k&list=PLkAUJkbhd-RjhKXPHQAZs_gBz-ZPYaNnu&index=6&ab_channel=KronosQuartet-Topic<br>https://www.youtube.com/watch?v=XBMLm9D7yoU&ab_channel=ShaneIvers-Topic<br>https://www.youtube.com/watch?v=6yeNQ487ACc&ab_channel=KeysofMoonMusic<br>https://www.youtube.com/watch?v=txoqe611j-o&ab_channel=KronosQuartet-To<br>https://pixabay.com/sound-effects/burningmountain-2-23681/<br>https://pixabay.com/sound-effects/water-in-stream-16801/<br>https://pixabay.com/sound-effects/calm-river-ambience-loop-125071/";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 5)
        {
            titleText.text = "Miscellaneous Sound Effects";
            middleBodyText.text = "Page Flip: https://pixabay.com/sound-effects/page-flip-47177/m/sound-effects/fx-light-90387/<br>Shield: https://pixabay.com/sound-effects/search/shield/<br>Cough: https://pixabay.com/sound-effects/cough-cough-103526/<br>Scroll Open: https://pixabay.com/sound-effects/old-heavy-parchment-107797/<br>Transition: https://pixabay.com/sound-effects/transition-base-121422/";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 6)
        {
            titleText.text = "Player Sound Effects";
            middleBodyText.text = "Coin: https://pixabay.com/sound-effects/dropping-single-coin-on-floor-2-38987/<br>Hiss: https://pixabay.com/sound-effects/snake-hissing-6092/<br>Hit: https://pixabay.com/sound-effects/big-pillow-hit-101877/<br>Venom: https://www.zapsplat.com/music/snake-spit-venom-2/<br>Fortune: https://pixabay.com/sound-effects/fx-light-90387/<br>Stab: https://pixabay.com/sound-effects/knife-stab-sound-effect-36354/";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 7)
        {
            titleText.text = "Player Sound Effects Continued";
            middleBodyText.text = "Wisdom: MAGIC_SPELL_Distinct_Thrust_Bright_Pad_stereo from Universal Sound FX<br>Crunch: BREAK_Fast_stereo from Universal Sound FX<br>Heartbeat: https://pixabay.com/sound-effects/heartbeat-6396/<br>Tea Pour: https://pixabay.com/sound-effects/tea-pouring-86194/<br>Gavel: https://pixabay.com/sound-effects/gavel-of-justice-124029/<br>Splash: https://pixabay.com/sound-effects/breeze-of-blood-122253/";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 8)
        {
            titleText.text = "Enemy Sound Effects";
            middleBodyText.text = "Throw Stones: SPLASH_Big_Noise_mono from Universal Sound FX<br>Body Slam: https://www.zapsplat.com/music/monster-impact-hit-and-kill/<br>Stubborn: https://www.zapsplat.com/music/big-creepy-monster-nasal-snarl-and-growl-1/<br>Wind Blow: https://pixabay.com/sound-effects/wind-blow-141288/<br>Rock Smash: https://pixabay.com/sound-effects/rock-smash-6304/";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 9)
        {
            titleText.text = "Enemy Sound Effects Continued";
            middleBodyText.text = "Reverberate: MAGIC_SPELL_Rumble_Spooky_Vibrato_Fade_In_Out_stereo from Universal Sound FX<br>Breath: Fire Breath | Royalty Free Music - Pixabay<br>Monsterize: Dark Magic Loop | Royalty Free Music - Pixabay<br>Breeze: howling wind | Royalty Free Music - Pixabay<br>Whoosh: Woosh_Low_Long01 | Royalty Free Music - Pixabay<br>Cyclone: Hurricane - Storm - Nature Sounds | Royalty Free Music - Pixabay";
            leftBodyText.text = "";
            rightBodyText.text = "";
            pageCount = -1;
        }
        // else if (pageCount == 3)
        // {
        //     titleText.text = "Fonts";
        //     middleBodyText.text = "";
        //     leftBodyText.text = "";
        //     rightBodyText.text = "";
        // }



    }
}
