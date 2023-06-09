using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject UICanvas;
    [SerializeField] TutorialSetup _TutorialSetup;
    
    public void Tutorial_1_Return()
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 0.1f);
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void Tutorial_2_Return()
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 0.1f);
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void Tutorial_3_Return()
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 0.1f);
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
