using UnityEngine;
using UnityEngine.SceneManagement;

public class PickDeckLevel_1Setup : MonoBehaviour
{
    public bool isFinished = false;

    private void Start()
    {
        //GameController.instance.SetCharacter(GameController.characterType.player, GameController.instance.GetCharacter("Bai Suzhen"));
        SoundManager.PlaySound("bgm_Yugen", 0.3f);
    }

    private void Update()
    {
        if (isFinished)
        {
            SceneManager.LoadScene("BattleLevel");
            isFinished = false;
        }
    }
}
