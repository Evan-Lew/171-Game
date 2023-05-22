using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryIntro : MonoBehaviour
{
    private void Awake()
    {
        GameController.instance.FadeIn();
        //GameController.instance.ChangeStoryBackground(1);
    }

    public void ToMap()
    {
        SceneManager.LoadScene("BattleMap");
    }

    public void StartBattle()
    {

        SceneManager.LoadScene("BattleLevel");
    }
}
