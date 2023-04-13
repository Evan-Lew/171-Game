using UnityEngine;
using UnityEngine.SceneManagement;

public class PickDeckLevel_1Setup : MonoBehaviour
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
            SceneManager.LoadScene("PickDeckLevel_2");
            isFinished = false;
        }
    }
}
