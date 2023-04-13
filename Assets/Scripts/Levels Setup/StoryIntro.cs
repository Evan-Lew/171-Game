using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryIntro : MonoBehaviour
{
    public void StartBattle()
    {
        SceneManager.LoadScene("BattleLevel");
    }
}
