using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryIntroSetup : MonoBehaviour
{
    private void Start()
    {
        GameController.instance.FadeIn();
        SoundManager.PlaySound("bgm_Sebastian", 0.2f);
        // Activate all story background game objects
        for (int i = 0; i < GameController.instance.storyBackgroundsList.Count; i++)
        {
            GameController.instance.storyBackgroundsList[i].SetActive(true);
        }
    }
}
