using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject UICanvas;
    [SerializeField] TutorialSetup _TutorialSetup;


    public void Tutorial_1_Return()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void Tutorial_2_Return()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void Tutorial_3_Return()
    {
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
