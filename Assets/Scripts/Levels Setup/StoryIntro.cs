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

        // Activate all story background game objects
        for (int i = 0; i < GameController.instance.storyBackgroundsList.Count; i++)
        {
            GameController.instance.storyBackgroundsList[i].SetActive(true);
        }
    }

    public void ToMap()
    {
        SceneManager.LoadScene("BattleMap");
    }

    public void StartBattle()
    {

        SceneManager.LoadScene("BattleLevel");
    }
    public void ChangeDecks()
    {
        SceneManager.LoadScene("PickDeckLevel_1");
    }
}
