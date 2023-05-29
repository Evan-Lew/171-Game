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
        SoundManager.PlaySound("bgm_River_Ambient", 0.15f);
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
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        SoundManager.bgmAudioSource.Stop();
        SceneManager.LoadScene("MainMenu");
    }
    
    public void Next()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        pageCount += 1;
        if (pageCount == 0)
        {
            titleText.text = "Team 10 - 171";
            leftBodyText.text = "Producer<br>Lead Programmer<br>Programmers<br><br>Art Director<br>Artists<br>Environment Design<br>Sound<br>Writers<br>Card Designers";
            rightBodyText.text = "Evan Lew<br>Laihong Xu<br>Alex Xie, Steven Ju,<br>Evan Lew, Chris Pau <br>Ed Gopez<br>Angela Yim & Lou Yonzon<br>Andy Eng<br>Derek Tran<br>Laihong & Ed<br>Alex & Steven";
            middleBodyText.text = "";
        }
        else if (pageCount == 1)
        {
            titleText.text = " Research";
            middleBodyText.text = "https://en.wikipedia.org/wiki/Legend_of_the_White_Snake<br><br>https://youtu.be/eEeeClBoqK0<br><br>https://youtu.be/mO6eMTKalRE";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 2)
        {
            titleText.text = "Art Style and Costume Inspirations";
            middleBodyText.text = "https://www.youtube.com/watch?v=8y9QnS_tMkY<br>http://www.asean-china-center.org/english/2010-05/26/c_13315960.htm<br>http://www.rjcc.or.kr/journal/article.php?code=37890<br>https://www.chinasilkmuseum.com/zggd/list_21.aspx";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 3)
        {
            titleText.text = "Fonts";
            middleBodyText.text = "Perpetua Font Family<br><br>Day Roman Font Family";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 4)
        {
            titleText.text = "Unity Packages";
            middleBodyText.text = "<u>Particles</u><br>https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-pack-vol-1-231405<br>https://assetstore.unity.com/packages/vfx/particles/spells/100-special-skills-effects-pack-171146<br><u>Sound Effects</u><br>https://assetstore.unity.com/packages/audio/sound-fx/universal-sound-fx-17256#description<br><u>Environment</u><br>https://assetstore.unity.com/packages/3d/environments/fantasy/dreamscape-nature-bundle-stylized-open-world-environment-232630";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 5)
        {
            titleText.text = "Miscellaneous Sound Effects";
            middleBodyText.text = "Page Flip: https://pixabay.com/sound-effects/page-flip-47177/m/sound-effects/fx-light-90387/";
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
            middleBodyText.text = "Wisdom: MAGIC_SPELL_Distinct_Thrust_Bright_Pad_stereo from Universal Sound FX<br>Crunch: BREAK_Fast_stereo from Universal Sound FX<br>Heartbeat: https://pixabay.com/sound-effects/heartbeat-6396/<br>Tea Pour: https://pixabay.com/sound-effects/tea-pouring-86194/";
            leftBodyText.text = "";
            rightBodyText.text = "";
        }
        else if (pageCount == 8)
        {
            titleText.text = "Enemy Sound Effects";
            middleBodyText.text = "Throw Stones: SPLASH_Big_Noise_mono from Universal Sound FX<br>Body Slam: https://www.zapsplat.com/music/monster-impact-hit-and-kill/<br>Stubborn: https://www.zapsplat.com/music/big-creepy-monster-nasal-snarl-and-growl-1/";
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
