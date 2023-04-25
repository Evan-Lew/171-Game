using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryIntro : MonoBehaviour
{
     public void ToMap()
    {

        SceneManager.LoadScene("BattleMap");
    }

    public void StartBattle()
    {

        SceneManager.LoadScene("BattleLevel");
    }
}
