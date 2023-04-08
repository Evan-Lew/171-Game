using UnityEngine;
using UnityEngine.SceneManagement;

public class Level02_Setup : MonoBehaviour
{
    public bool isFinished = false;

    private void Start()
    {
        GameController.instance.SetCharacter(GameController.characterType.player, GameController.instance.GetCharacter("Bai Suzhen"));
    }

    private void Update()
    {
        if (isFinished)
        {
            SceneManager.LoadScene("Level03");
            isFinished = false;
        }
    }
}
